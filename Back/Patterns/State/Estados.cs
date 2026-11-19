namespace ApiCycleOn.Patterns.State;

public static class EstadosUsuario
{
    public const string Activo = "ACTIVO", Bloqueado = "BLOQUEADO", Eliminado = "ELIMINADO";
}

public static class EstadosBicicleta
{
    public const string Disponible = "DISPONIBLE",
        Reservada = "RESERVADA",
        EnPrestamo = "EN_PRESTAMO",
        EnMantenimiento = "EN_MANTENIMIENTO",
        DadaDeBaja = "DADA_DE_BAJA";
}

public static class EstadosReserva
{
    public const string Vigente = "VIGENTE",
        Utilizada = "UTILIZADA",
        Cancelada = "CANCELADA",
        Expirada = "EXPIRADA";
}

public static class EstadosPago
{
    public const string Pendiente = "PENDIENTE",
        EnProceso = "EN_PROCESO",
        Confirmado = "CONFIRMADO",
        Expirado = "EXPIRADO",
        Bloqueado = "BLOQUEADO";
}

public static class EstadosMantenimiento
{
    public const string Programado = "PROGRAMADO",
        EnEjecucion = "EN_EJECUCION",
        Finalizado = "FINALIZADO",
        Cancelado = "CANCELADO";
}

public static class EstadosEstacion
{
    public const string Operativa = "OPERATIVA",
        SinStock = "SIN_STOCK",
        Saturada = "SATURADA",
        Inactiva = "INACTIVA";
}

public static class EstadosActivable // Tarifa
{
    public const string Activa = "ACTIVA", Inactiva = "INACTIVA";
}

public static class EstadosSancion
{
    public const string PendientePago = "PENDIENTE_PAGO",
        ActivaTemporal = "ACTIVA_TEMPORAL",
        ActivaPermanente = "ACTIVA_PERMANENTE",
        Cumplida = "CUMPLIDA",
        Revocada = "REVOCADA";
}

public static class EstadosNotificacion
{
    public const string Pendiente = "PENDIENTE", Enviada = "ENVIADA", Fallida = "FALLIDA";
}