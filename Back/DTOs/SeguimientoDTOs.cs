using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// RegistrarUbicacion() - la app envía la posición durante el préstamo
public record RegistrarUbicacionRequest(
    [property: Range(1, int.MaxValue)] int IdPrestamo,
    [property: Range(-90, 90)] decimal Latitud,
    [property: Range(-180, 180)] decimal Longitud);

#endregion

#region Responses

// ConsultarUbicacionActual() devuelve el último registro
public record GetUbicacionResponse(
    int IdSeguimiento,
    int IdPrestamo,
    decimal Latitud,
    decimal Longitud,
    DateTime FechaHora);

#endregion