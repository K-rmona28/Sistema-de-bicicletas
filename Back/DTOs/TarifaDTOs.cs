using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// CalcularValorServicio() / ConsultarCostoEstimado() del cliente
// Si IdTarifa es null se usa la tarifa vigente
public record CalcularValorServicioRequest(
    [property: Range(1, 1440)] int DuracionMinutos,
    int? IdTarifa);

public record CrearTarifaRequest(
    [property: Range(0.01, 999999999)] decimal ValorHora,
    DateTime VigenciaInicio,
    DateTime? VigenciaFin) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (VigenciaFin.HasValue && VigenciaFin <= VigenciaInicio)
            yield return new ValidationResult("La vigencia final debe ser posterior a la inicial.", [nameof(VigenciaFin)]);
    }
}

// ModificarTarifa() - idTarifa va en la ruta
public record ModificarTarifaRequest(
    [property: Range(0.01, 999999999)] decimal ValorHora,
    [property: Required, StringLength(20)] string Estado,
    DateTime VigenciaInicio,
    DateTime? VigenciaFin) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (VigenciaFin.HasValue && VigenciaFin <= VigenciaInicio)
            yield return new ValidationResult("La vigencia final debe ser posterior a la inicial.", [nameof(VigenciaFin)]);
    }
}

#endregion

#region Responses

// MostrarTarifa()
public record GetTarifaResponse(
    int IdTarifa,
    decimal ValorHora,
    string Estado,
    DateTime VigenciaInicio,
    DateTime? VigenciaFin);

public record GetValorServicioResponse(
    int IdTarifa,
    decimal ValorHora,
    int DuracionMinutos,
    decimal ValorTotal);

#endregion