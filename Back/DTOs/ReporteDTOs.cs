using System.ComponentModel.DataAnnotations;
using ApiCycleOn.Enums;

namespace ApiCycleOn.DTOs;

#region Requests

// RegistrarReporte() / SolicitarReporte() del cliente - el usuario sale del token
// Debe indicar al menos una estación o una bicicleta (igual que el CHECK de la base)
public record RegistrarReporteRequest(
    [property: Required, StringLength(50)] string Tipo,
    [property: Required, StringLength(500)] string Descripcion,
    int? IdEstacion,
    int? IdBicicleta) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IdEstacion is null && IdBicicleta is null)
            yield return new ValidationResult("Debes indicar una estación o una bicicleta.", [nameof(IdEstacion), nameof(IdBicicleta)]);
    }
}

// ManejoDeReportes() - idReporte va en la ruta
public record ActualizarEstadoReporteRequest(
    EstadoReporte Estado);

public record FiltroReportesRequest(
    EstadoReporte? Estado,
    [property: StringLength(50)] string? Tipo,
    DateTime? Desde,
    DateTime? Hasta,
    [property: Range(1, int.MaxValue)] int Page = 1,
    [property: Range(1, 100)] int PageSize = 20);

#endregion

#region Responses

public record GetReporteResponse(
    int IdReporte,
    string Tipo,
    string Descripcion,
    DateTime FechaHora,
    int? IdEstacion,
    int IdUsuario,
    int? IdBicicleta,
    EstadoReporte Estado);

#endregion
