using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// RegistrarMantenimiento() y MarcarMantenimiento() de Bicicleta
public record RegistrarMantenimientoRequest(
    [property: Range(1, int.MaxValue)] int IdBicicleta,
    [property: Required, StringLength(50)] string Tipo,
    DateTime FechaProgramada,
    DateTime? FechaHoraInicio,
    [property: StringLength(500)] string? Descripcion);

// FinalizarMantenimiento() - idMantenimiento va en la ruta
public record FinalizarMantenimientoRequest(
    DateTime? FechaFin,                 // si es null el servicio usa la fecha actual
    [property: StringLength(500)] string? Descripcion);

#endregion

#region Responses

public record GetMantenimientoResponse(
    int IdMantenimiento,
    int IdBicicleta,
    string Tipo,
    DateTime FechaProgramada,
    DateTime? FechaHoraInicio,
    DateTime? FechaFin,
    string? Descripcion,
    string Estado);

#endregion