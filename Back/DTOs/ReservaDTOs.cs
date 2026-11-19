using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// CrearReserva() - el cliente sale del token
public record CrearReservaRequest(
    [property: Range(1, int.MaxValue)] int IdEstacion,
    [property: Range(1, int.MaxValue)] int IdBicicleta);

// CancelarReserva() - idReserva va en la ruta
public record CancelarReservaRequest(
    [property: StringLength(250)] string? Motivo);

#endregion

#region Responses

public record GetReservaResponse(
    int IdReserva,
    int IdUsuario,
    int IdEstacion,
    string NombreEstacion,
    int IdBicicleta,
    DateTime FechaHoraReserva,
    string Estado);

#endregion