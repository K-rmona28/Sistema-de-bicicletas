-- ============================================================
-- ApiCycleOn - Init de la base de datos de auditoría
-- Motor: SQL Server
-- Idempotente: se puede ejecutar varias veces sin error.
-- Debe ejecutarse ANTES del trigger TR_CopiarAuditoria de la BD general.
-- ============================================================

IF DB_ID(N'AuditoriaCycleOn') IS NULL
BEGIN
    CREATE DATABASE AuditoriaCycleOn;
END
GO

USE AuditoriaCycleOn;
GO

IF OBJECT_ID(N'dbo.RegistroAuditoria', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RegistroAuditoria
    (
        idAuditoria        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        idUsuario          INT               NOT NULL,
        Accion             NVARCHAR(50)      NOT NULL,
        EntidadAfectada    NVARCHAR(60)      NOT NULL,
        idRegistroAfectado INT               NULL,
        TipoEvento         NVARCHAR(50)      NOT NULL,
        FechaHora          DATETIME2(0)      NOT NULL,
        DescripcionCambio  NVARCHAR(MAX)     NULL
    );
END
GO
