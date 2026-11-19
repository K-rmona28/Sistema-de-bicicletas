namespace ApiCycleOn.DTOs;

#region Requests

// VerVelocidadMedia() y ConsultarHistorial() usan RangoFechasRequest (CommonDTOs)

#endregion

#region Responses

public record GetVelocidadMediaResponse(
    double VelocidadMediaKmH,
    double DistanciaTotalKm,
    int TiempoTotalMinutos);

public record GetEstadisticaResponse(
    int IdEstadistica,
    int Historial,
    int TiempoDeUso,               // minutos
    decimal DistanciaRecorrida,    // km
    int Estaciones);

#endregion