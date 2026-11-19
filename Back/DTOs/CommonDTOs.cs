using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

public record PagedRequest(
    [property: Range(1, int.MaxValue)] int Page = 1,
    [property: Range(1, 100)] int PageSize = 20);

public record RangoFechasRequest(DateTime? Desde, DateTime? Hasta) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Desde.HasValue && Hasta.HasValue && Hasta < Desde)
            yield return new ValidationResult("'Hasta' no puede ser anterior a 'Desde'.", [nameof(Hasta)]);
    }
}

#endregion

#region Responses

public record GetMensajeResponse(string Mensaje);

public record GetPagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
}

#endregion