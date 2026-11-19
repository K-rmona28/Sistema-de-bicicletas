using System.ComponentModel.DataAnnotations;
using ApiCycleOn.Enums;

namespace ApiCycleOn.DTOs;

#region Requests

// El cliente sale del token; la tarifa vigente la elige el servicio
public record CrearPrestamoRequest(
    int? IdReserva,
    [property: Range(1, int.MaxValue)] int IdBicicleta,
    [property: Range(1, int.MaxValue)] int EstacionOrigen);

// idPrestamo va en la ruta
public record FinalizarPrestamoRequest(
    [property: Range(1, int.MaxValue)] int EstacionDestino);

public record CancelarPrestamoRequest(
    [property: StringLength(250)] string? Motivo);

// VerHistorial() (cliente) y ConsultarHistorialUsuario(idUsuario) (administrador)
public record FiltroHistorialRequest(
    DateTime? Desde,
    DateTime? Hasta,
    EstadoPrestamo? Estado,
    [property: Range(1, int.MaxValue)] int Page = 1,
    [property: Range(1, 100)] int PageSize = 20);

#endregion

#region Responses

public record GetPrestamoResponse(
    int IdPrestamo,
    int IdUsuario,
    int? IdReserva,
    int IdTarifa,
    decimal ValorHora,
    int IdBicicleta,
    string Serial,
    DateTime FechaHoraInicio,
    DateTime? FechaHoraFin,
    decimal? CostoTotal,
    EstadoPrestamo Estado,
    int? TiempoUso,                 // minutos
    int EstacionOrigen,
    string NombreEstacionOrigen,
    int? EstacionDestino,
    string? NombreEstacionDestino);

#endregion