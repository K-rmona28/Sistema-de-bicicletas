/* =====================================================================
   SISTEMA DE RESERVA Y PRESTAMO DE BICICLETAS CycleOn
   Script de creacion de base de datos - SQL Server (SSMS)
   Basado en el diagrama de clases.
   ===================================================================== */

IF DB_ID(N'CycleOnDB') IS NULL
    CREATE DATABASE CycleOnDB;
GO

USE CycleOnDB;
GO

/* =====================================================================
   1. USUARIOS (clase abstracta + especializacion)
   ===================================================================== */

CREATE TABLE dbo.Usuario (
    idUsuario         INT IDENTITY(1,1) NOT NULL,
    Nombre            NVARCHAR(80)  NOT NULL,
    Apellido          NVARCHAR(80)  NOT NULL,
    Email             NVARCHAR(150) NOT NULL,
    Pass              NVARCHAR(255) NOT NULL,   -- guardar HASH, nunca texto plano
    Estado            NVARCHAR(20)  NOT NULL CONSTRAINT DF_Usuario_Estado DEFAULT N'ACTIVO',
    Telefono          NVARCHAR(20)  NULL,
    FechaDeNacimiento DATE          NOT NULL,
    -- Edad derivada: siempre correcta, no hay que actualizarla
    Edad AS (
        DATEDIFF(YEAR, FechaDeNacimiento, GETDATE())
        - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, FechaDeNacimiento, GETDATE()), FechaDeNacimiento) > GETDATE()
               THEN 1 ELSE 0 END
    ),
    FechaDeRegistro   DATETIME2(0)  NOT NULL CONSTRAINT DF_Usuario_FechaReg DEFAULT SYSDATETIME(),
    IntentosFallidos  INT           NOT NULL CONSTRAINT DF_Usuario_Intentos DEFAULT 0,
    CONSTRAINT PK_Usuario PRIMARY KEY (idUsuario),
    CONSTRAINT UQ_Usuario_Email UNIQUE (Email),
    CONSTRAINT CK_Usuario_Intentos CHECK (IntentosFallidos >= 0)
);
GO

-- Especializacion: Administrador (comparte PK con Usuario)
CREATE TABLE dbo.Administrador (
    idUsuario INT          NOT NULL,
    Cargo     NVARCHAR(80) NOT NULL,
    CONSTRAINT PK_Administrador PRIMARY KEY (idUsuario),
    CONSTRAINT FK_Administrador_Usuario FOREIGN KEY (idUsuario)
        REFERENCES dbo.Usuario (idUsuario) ON DELETE CASCADE
);
GO

-- Especializacion: Cliente
CREATE TABLE dbo.Cliente (
    idUsuario                INT          NOT NULL,
    NombreContactoEmergencia NVARCHAR(120) NOT NULL,
    TelContactoEmergencia    NVARCHAR(20)  NOT NULL,
    MaximoPrestamos          INT          NOT NULL CONSTRAINT DF_Cliente_MaxPrest DEFAULT 1,
    CONSTRAINT PK_Cliente PRIMARY KEY (idUsuario),
    CONSTRAINT FK_Cliente_Usuario FOREIGN KEY (idUsuario)
        REFERENCES dbo.Usuario (idUsuario) ON DELETE CASCADE,
    CONSTRAINT CK_Cliente_MaxPrestamos CHECK (MaximoPrestamos <= 1)   -- {MaximoPrestamos <= 1}
);
GO

/* =====================================================================
   2. ESTACIONES, BICICLETAS, TARIFAS
   ===================================================================== */

CREATE TABLE dbo.Estacion (
    idEstacion INT IDENTITY(1,1) NOT NULL,
    Nombre     NVARCHAR(100) NOT NULL,
    Ubicacion  NVARCHAR(200) NOT NULL,
    Capacidad  INT           NOT NULL,
    Estado     NVARCHAR(20)  NOT NULL CONSTRAINT DF_Estacion_Estado DEFAULT N'ACTIVA',
    CONSTRAINT PK_Estacion PRIMARY KEY (idEstacion),
    CONSTRAINT CK_Estacion_Capacidad CHECK (Capacidad > 0)
);
GO

CREATE TABLE dbo.Bicicleta (
    idBicicleta   INT IDENTITY(1,1) NOT NULL,
    idEstacion    INT           NULL,        -- NULL = la bici esta en prestamo / fuera de estacion
    Tipo          NVARCHAR(50)  NOT NULL,
    Tamano        NVARCHAR(20)  NOT NULL,    -- (Tamaño) sin ñ para evitar problemas en el back
    Especificacion NVARCHAR(250) NULL,
    Estado        NVARCHAR(20)  NOT NULL CONSTRAINT DF_Bicicleta_Estado DEFAULT N'DISPONIBLE',
    Marca         NVARCHAR(60)  NOT NULL,
    Modelo        NVARCHAR(60)  NOT NULL,
    Serial        NVARCHAR(60)  NOT NULL,
    Valor         DECIMAL(12,2) NOT NULL,
    CONSTRAINT PK_Bicicleta PRIMARY KEY (idBicicleta),
    CONSTRAINT UQ_Bicicleta_Serial UNIQUE (Serial),
    CONSTRAINT CK_Bicicleta_Valor CHECK (Valor >= 0),
    CONSTRAINT FK_Bicicleta_Estacion FOREIGN KEY (idEstacion)
        REFERENCES dbo.Estacion (idEstacion)
);
GO

CREATE TABLE dbo.Tarifa (
    idTarifa       INT IDENTITY(1,1) NOT NULL,
    ValorHora      DECIMAL(12,2) NOT NULL,
    Estado         NVARCHAR(20)  NOT NULL CONSTRAINT DF_Tarifa_Estado DEFAULT N'ACTIVA',
    VigenciaInicio DATETIME2(0)  NOT NULL,
    VigenciaFin    DATETIME2(0)  NULL,
    CONSTRAINT PK_Tarifa PRIMARY KEY (idTarifa),
    CONSTRAINT CK_Tarifa_Valor CHECK (ValorHora >= 0),
    CONSTRAINT CK_Tarifa_Vigencia CHECK (VigenciaFin IS NULL OR VigenciaFin > VigenciaInicio)
);
GO

/* =====================================================================
   3. RESERVAS Y PRESTAMOS
   ===================================================================== */

CREATE TABLE dbo.Reserva (
    idReserva        INT IDENTITY(1,1) NOT NULL,
    idUsuario        INT          NOT NULL,   -- Cliente
    idEstacion       INT          NOT NULL,
    idBicicleta      INT          NOT NULL,
    FechaHoraReserva DATETIME2(0) NOT NULL CONSTRAINT DF_Reserva_Fecha DEFAULT SYSDATETIME(),
    Estado           NVARCHAR(20) NOT NULL CONSTRAINT DF_Reserva_Estado DEFAULT N'ACTIVA',
    CONSTRAINT PK_Reserva PRIMARY KEY (idReserva),
    CONSTRAINT FK_Reserva_Cliente   FOREIGN KEY (idUsuario)   REFERENCES dbo.Cliente (idUsuario),
    CONSTRAINT FK_Reserva_Estacion  FOREIGN KEY (idEstacion)  REFERENCES dbo.Estacion (idEstacion),
    CONSTRAINT FK_Reserva_Bicicleta FOREIGN KEY (idBicicleta) REFERENCES dbo.Bicicleta (idBicicleta)
);
GO

CREATE TABLE dbo.Prestamo (
    idPrestamo      INT IDENTITY(1,1) NOT NULL,
    idUsuario       INT          NOT NULL,   -- Cliente
    idReserva       INT          NULL,       -- un prestamo puede nacer de una reserva (0:1)
    idTarifa        INT          NOT NULL,
    idBicicleta     INT          NOT NULL,
    FechaHoraInicio DATETIME2(0) NOT NULL CONSTRAINT DF_Prestamo_Inicio DEFAULT SYSDATETIME(),
    FechaHoraFin    DATETIME2(0) NULL,
    CostoTotal      DECIMAL(12,2) NULL,      -- /CostoTotal: lo calcula el back al finalizar
    Estado          NVARCHAR(20) NOT NULL CONSTRAINT DF_Prestamo_Estado DEFAULT N'ACEPTADO',
    -- /TiempoUso derivado: minutos de uso
    TiempoUso AS (DATEDIFF(MINUTE, FechaHoraInicio, FechaHoraFin)) PERSISTED,
    EstacionOrigen  INT          NOT NULL,
    EstacionDestino INT          NULL,       -- se llena al devolver la bici
    CONSTRAINT PK_Prestamo PRIMARY KEY (idPrestamo),
    CONSTRAINT FK_Prestamo_Cliente   FOREIGN KEY (idUsuario)       REFERENCES dbo.Cliente (idUsuario),
    CONSTRAINT FK_Prestamo_Reserva   FOREIGN KEY (idReserva)       REFERENCES dbo.Reserva (idReserva),
    CONSTRAINT FK_Prestamo_Tarifa    FOREIGN KEY (idTarifa)        REFERENCES dbo.Tarifa (idTarifa),
    CONSTRAINT FK_Prestamo_Bicicleta FOREIGN KEY (idBicicleta)     REFERENCES dbo.Bicicleta (idBicicleta),
    CONSTRAINT FK_Prestamo_EstOrigen  FOREIGN KEY (EstacionOrigen)  REFERENCES dbo.Estacion (idEstacion),
    CONSTRAINT FK_Prestamo_EstDestino FOREIGN KEY (EstacionDestino) REFERENCES dbo.Estacion (idEstacion),
    -- enumeration EstadoPrestamo
    CONSTRAINT CK_Prestamo_Estado CHECK (Estado IN (N'ACEPTADO', N'EN_PROCESO', N'FINALIZADO', N'CANCELADO')),
    CONSTRAINT CK_Prestamo_Fechas CHECK (FechaHoraFin IS NULL OR FechaHoraFin >= FechaHoraInicio),
    CONSTRAINT CK_Prestamo_Costo  CHECK (CostoTotal IS NULL OR CostoTotal >= 0)
);
GO

-- Reserva 0:1 <-> 0:1 Prestamo: una reserva solo puede generar un prestamo
CREATE UNIQUE INDEX UX_Prestamo_Reserva
    ON dbo.Prestamo (idReserva) WHERE idReserva IS NOT NULL;

-- Regla {MaximoPrestamos <= 1}: un cliente solo puede tener 1 prestamo activo a la vez
CREATE UNIQUE INDEX UX_Prestamo_ClienteActivo
    ON dbo.Prestamo (idUsuario) WHERE Estado IN (N'ACEPTADO', N'EN_PROCESO');

-- Una bicicleta no puede estar en 2 prestamos activos a la vez
CREATE UNIQUE INDEX UX_Prestamo_BicicletaActiva
    ON dbo.Prestamo (idBicicleta) WHERE Estado IN (N'ACEPTADO', N'EN_PROCESO');
GO

-- Composicion: Seguimiento no existe sin Prestamo
CREATE TABLE dbo.Seguimiento (
    idSeguimiento INT IDENTITY(1,1) NOT NULL,
    idPrestamo    INT           NOT NULL,
    Latitud       DECIMAL(9,6)  NOT NULL,
    Longitud      DECIMAL(9,6)  NOT NULL,
    FechaHora     DATETIME2(0)  NOT NULL CONSTRAINT DF_Seguimiento_Fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Seguimiento PRIMARY KEY (idSeguimiento),
    CONSTRAINT FK_Seguimiento_Prestamo FOREIGN KEY (idPrestamo)
        REFERENCES dbo.Prestamo (idPrestamo) ON DELETE CASCADE,
    CONSTRAINT CK_Seguimiento_Lat CHECK (Latitud  BETWEEN -90  AND 90),
    CONSTRAINT CK_Seguimiento_Lon CHECK (Longitud BETWEEN -180 AND 180)
);
GO

/* =====================================================================
   4. PAGOS Y FACTURAS
   ===================================================================== */

CREATE TABLE dbo.Pago (
    idPago        INT IDENTITY(1,1) NOT NULL,
    idPrestamo    INT           NOT NULL,
    MetodoPago    NVARCHAR(30)  NOT NULL,
    Monto         DECIMAL(12,2) NOT NULL,
    FechaHoraPago DATETIME2(0)  NOT NULL CONSTRAINT DF_Pago_Fecha DEFAULT SYSDATETIME(),
    Estado        NVARCHAR(20)  NOT NULL CONSTRAINT DF_Pago_Estado DEFAULT N'PENDIENTE',
    CONSTRAINT PK_Pago PRIMARY KEY (idPago),
    CONSTRAINT UQ_Pago_Prestamo UNIQUE (idPrestamo),          -- Prestamo 1 : Pago 0:1
    CONSTRAINT FK_Pago_Prestamo FOREIGN KEY (idPrestamo) REFERENCES dbo.Prestamo (idPrestamo),
    CONSTRAINT CK_Pago_Monto CHECK (Monto >= 0)
);
GO

CREATE TABLE dbo.Factura (
    idFactura     INT IDENTITY(1,1) NOT NULL,
    idUsuario     INT           NOT NULL,   -- (IdCliente)
    idPago        INT           NOT NULL,
    FechaEmision  DATETIME2(0)  NOT NULL CONSTRAINT DF_Factura_Fecha DEFAULT SYSDATETIME(),
    MontoTotal    DECIMAL(12,2) NOT NULL,
    CodigoFactura NVARCHAR(40)  NOT NULL,
    CONSTRAINT PK_Factura PRIMARY KEY (idFactura),
    CONSTRAINT UQ_Factura_Pago   UNIQUE (idPago),             -- relacion 1 : 1 con Pago
    CONSTRAINT UQ_Factura_Codigo UNIQUE (CodigoFactura),
    CONSTRAINT FK_Factura_Cliente FOREIGN KEY (idUsuario) REFERENCES dbo.Cliente (idUsuario),
    CONSTRAINT FK_Factura_Pago    FOREIGN KEY (idPago)    REFERENCES dbo.Pago (idPago),
    CONSTRAINT CK_Factura_Monto CHECK (MontoTotal >= 0)
);
GO

/* =====================================================================
   5. REPORTES, EVIDENCIAS Y SANCIONES
   ===================================================================== */

CREATE TABLE dbo.Reporte (
    idReporte   INT IDENTITY(1,1) NOT NULL,
    Tipo        NVARCHAR(50)  NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaHora   DATETIME2(0)  NOT NULL CONSTRAINT DF_Reporte_Fecha DEFAULT SYSDATETIME(),
    idEstacion  INT           NULL,
    idUsuario   INT           NOT NULL,
    idBicicleta INT           NULL,
    Estado      NVARCHAR(20)  NOT NULL CONSTRAINT DF_Reporte_Estado DEFAULT N'ABIERTO',
    CONSTRAINT PK_Reporte PRIMARY KEY (idReporte),
    CONSTRAINT FK_Reporte_Estacion  FOREIGN KEY (idEstacion)  REFERENCES dbo.Estacion (idEstacion),
    CONSTRAINT FK_Reporte_Usuario   FOREIGN KEY (idUsuario)   REFERENCES dbo.Usuario (idUsuario),
    CONSTRAINT FK_Reporte_Bicicleta FOREIGN KEY (idBicicleta) REFERENCES dbo.Bicicleta (idBicicleta),
    -- enumeration EstadoReporte
    CONSTRAINT CK_Reporte_Estado CHECK (Estado IN (N'ABIERTO', N'EN_CURSO', N'CERRADO')),
    -- un reporte debe referirse a una estacion o a una bicicleta (o a ambas)
    CONSTRAINT CK_Reporte_Objetivo CHECK (idEstacion IS NOT NULL OR idBicicleta IS NOT NULL)
);
GO

-- Composicion: Evidencia no existe sin Reporte
CREATE TABLE dbo.Evidencia (
    NumeroEvidencia INT IDENTITY(1,1) NOT NULL,
    idReporte       INT           NOT NULL,
    RutaArchivo     NVARCHAR(300) NOT NULL,
    Tipo            NVARCHAR(30)  NOT NULL,
    FechaHoraCarga  DATETIME2(0)  NOT NULL CONSTRAINT DF_Evidencia_Fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Evidencia PRIMARY KEY (NumeroEvidencia),
    CONSTRAINT FK_Evidencia_Reporte FOREIGN KEY (idReporte)
        REFERENCES dbo.Reporte (idReporte) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Sancion (
    idSancion           INT IDENTITY(1,1) NOT NULL,
    idUsuario           INT           NOT NULL,
    idPrestamo          INT           NULL,
    idReporte           INT           NULL,   -- Reporte 0:1
    Motivo              NVARCHAR(200) NOT NULL,
    FechaHora           DATETIME2(0)  NOT NULL CONSTRAINT DF_Sancion_Fecha DEFAULT SYSDATETIME(),
    Estado              NVARCHAR(20)  NOT NULL CONSTRAINT DF_Sancion_Estado DEFAULT N'ACTIVA',
    Descripcion         NVARCHAR(500) NULL,
    TipoDePenalizacion  NVARCHAR(50)  NOT NULL,
    CONSTRAINT PK_Sancion PRIMARY KEY (idSancion),
    CONSTRAINT FK_Sancion_Usuario  FOREIGN KEY (idUsuario)  REFERENCES dbo.Usuario (idUsuario),
    CONSTRAINT FK_Sancion_Prestamo FOREIGN KEY (idPrestamo) REFERENCES dbo.Prestamo (idPrestamo),
    CONSTRAINT FK_Sancion_Reporte  FOREIGN KEY (idReporte)  REFERENCES dbo.Reporte (idReporte)
);
GO

/* =====================================================================
   6. MANTENIMIENTO Y MOVIMIENTOS
   ===================================================================== */

CREATE TABLE dbo.Mantenimiento (
    idMantenimiento  INT IDENTITY(1,1) NOT NULL,
    idBicicleta      INT           NOT NULL,
    Tipo             NVARCHAR(50)  NOT NULL,
    FechaProgramada  DATETIME2(0)  NOT NULL,
    FechaHoraInicio  DATETIME2(0)  NULL,
    FechaFin         DATETIME2(0)  NULL,
    Descripcion      NVARCHAR(500) NULL,
    Estado           NVARCHAR(20)  NOT NULL CONSTRAINT DF_Mantenimiento_Estado DEFAULT N'PROGRAMADO',
    CONSTRAINT PK_Mantenimiento PRIMARY KEY (idMantenimiento),
    CONSTRAINT FK_Mantenimiento_Bicicleta FOREIGN KEY (idBicicleta) REFERENCES dbo.Bicicleta (idBicicleta),
    CONSTRAINT CK_Mantenimiento_Fechas CHECK (FechaFin IS NULL OR FechaHoraInicio IS NULL OR FechaFin >= FechaHoraInicio)
);
GO

CREATE TABLE dbo.Movimiento (
    idMovimiento      INT IDENTITY(1,1) NOT NULL,
    idUsuario         INT           NOT NULL,   -- Administrador que lo realiza
    idEstacionOrigen  INT           NOT NULL,
    idEstacionDestino INT           NOT NULL,
    FechaHora         DATETIME2(0)  NOT NULL CONSTRAINT DF_Movimiento_Fecha DEFAULT SYSDATETIME(),
    Motivo            NVARCHAR(250) NOT NULL,
    CONSTRAINT PK_Movimiento PRIMARY KEY (idMovimiento),
    CONSTRAINT FK_Movimiento_Admin    FOREIGN KEY (idUsuario)         REFERENCES dbo.Administrador (idUsuario),
    CONSTRAINT FK_Movimiento_Origen   FOREIGN KEY (idEstacionOrigen)  REFERENCES dbo.Estacion (idEstacion),
    CONSTRAINT FK_Movimiento_Destino  FOREIGN KEY (idEstacionDestino) REFERENCES dbo.Estacion (idEstacion),
    CONSTRAINT CK_Movimiento_Estaciones CHECK (idEstacionOrigen <> idEstacionDestino)
);
GO

-- Composicion: DetalleMovimiento no existe sin Movimiento
CREATE TABLE dbo.DetalleMovimiento (
    idMovimiento INT NOT NULL,
    idBicicleta  INT NOT NULL,
    CONSTRAINT PK_DetalleMovimiento PRIMARY KEY (idMovimiento, idBicicleta),
    CONSTRAINT FK_DetMov_Movimiento FOREIGN KEY (idMovimiento)
        REFERENCES dbo.Movimiento (idMovimiento) ON DELETE CASCADE,
    CONSTRAINT FK_DetMov_Bicicleta  FOREIGN KEY (idBicicleta)
        REFERENCES dbo.Bicicleta (idBicicleta)
);
GO

/* =====================================================================
   7. AUDITORIA Y NOTIFICACIONES
   ===================================================================== */

CREATE TABLE dbo.RegistroAuditoria (
    idAuditoria        INT IDENTITY(1,1) NOT NULL,
    idUsuario          INT           NOT NULL,
    Accion             NVARCHAR(50)  NOT NULL,
    EntidadAfectada    NVARCHAR(60)  NOT NULL,
    idRegistroAfectado INT           NULL,
    TipoEvento         NVARCHAR(50)  NOT NULL,
    FechaHora          DATETIME2(0)  NOT NULL CONSTRAINT DF_Auditoria_Fecha DEFAULT SYSDATETIME(),
    DescripcionCambio  NVARCHAR(MAX) NULL,
    CONSTRAINT PK_RegistroAuditoria PRIMARY KEY (idAuditoria),
    CONSTRAINT FK_Auditoria_Usuario FOREIGN KEY (idUsuario) REFERENCES dbo.Usuario (idUsuario)
);
GO

CREATE TABLE dbo.NotificacionEmail (
    idNotificacion INT IDENTITY(1,1) NOT NULL,
    idUsuario      INT           NOT NULL,
    Mensaje        NVARCHAR(1000) NOT NULL,
    FechaHora      DATETIME2(0)  NOT NULL CONSTRAINT DF_NotEmail_Fecha DEFAULT SYSDATETIME(),
    Estado         NVARCHAR(20)  NOT NULL CONSTRAINT DF_NotEmail_Estado DEFAULT N'PENDIENTE',
    CONSTRAINT PK_NotificacionEmail PRIMARY KEY (idNotificacion),
    CONSTRAINT FK_NotEmail_Usuario FOREIGN KEY (idUsuario) REFERENCES dbo.Usuario (idUsuario)
);
GO

CREATE TABLE dbo.NotificacionWhatsApp (
    idNotificacion INT IDENTITY(1,1) NOT NULL,
    idUsuario      INT           NOT NULL,
    Mensaje        NVARCHAR(1000) NOT NULL,
    FechaHora      DATETIME2(0)  NOT NULL CONSTRAINT DF_NotWa_Fecha DEFAULT SYSDATETIME(),
    Estado         NVARCHAR(20)  NOT NULL CONSTRAINT DF_NotWa_Estado DEFAULT N'PENDIENTE',
    CONSTRAINT PK_NotificacionWhatsApp PRIMARY KEY (idNotificacion),
    CONSTRAINT FK_NotWa_Usuario FOREIGN KEY (idUsuario) REFERENCES dbo.Usuario (idUsuario)
);
GO

/* =====================================================================
   8. ESTADISTICA (sin relaciones en el diagrama)
   ===================================================================== */

CREATE TABLE dbo.Estadistica (
    idEstadistica      INT IDENTITY(1,1) NOT NULL,
    Historial          INT           NOT NULL,
    TiempoDeUso        INT           NOT NULL,   -- minutos
    DistanciaRecorrida DECIMAL(10,2) NOT NULL,   -- km
    Estaciones         INT           NOT NULL,
    CONSTRAINT PK_Estadistica PRIMARY KEY (idEstadistica)
);
GO

/* =====================================================================
   9. INDICES PARA LAS CONSULTAS MAS COMUNES
   ===================================================================== */

CREATE INDEX IX_Bicicleta_Estacion      ON dbo.Bicicleta (idEstacion);
CREATE INDEX IX_Reserva_Usuario         ON dbo.Reserva (idUsuario);
CREATE INDEX IX_Reserva_Bicicleta       ON dbo.Reserva (idBicicleta);
CREATE INDEX IX_Reserva_Estacion        ON dbo.Reserva (idEstacion);
CREATE INDEX IX_Prestamo_Tarifa         ON dbo.Prestamo (idTarifa);
CREATE INDEX IX_Seguimiento_Prestamo    ON dbo.Seguimiento (idPrestamo, FechaHora);
CREATE INDEX IX_Reporte_Bicicleta       ON dbo.Reporte (idBicicleta);
CREATE INDEX IX_Reporte_Usuario         ON dbo.Reporte (idUsuario);
CREATE INDEX IX_Sancion_Usuario         ON dbo.Sancion (idUsuario);
CREATE INDEX IX_Mantenimiento_Bicicleta ON dbo.Mantenimiento (idBicicleta);
CREATE INDEX IX_Auditoria_Usuario       ON dbo.RegistroAuditoria (idUsuario, FechaHora);
CREATE INDEX IX_NotEmail_Usuario        ON dbo.NotificacionEmail (idUsuario);
CREATE INDEX IX_NotWa_Usuario           ON dbo.NotificacionWhatsApp (idUsuario);
GO

PRINT 'Base de datos BicicletasDB creada correctamente.';
GO
