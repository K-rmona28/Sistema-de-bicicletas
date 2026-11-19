using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// RegistrarMovimiento() - el administrador sale del token
public record RegistrarMovimientoRequest(
    [property: Range(1, int.MaxValue)] int IdEstacionOrigen,
    [property: Range(1, int.MaxValue)] int IdEstacionDestino,
    [property: Required, StringLength(250)] string Motivo,
    [property: Required, MinLength(1)] List<int> IdsBicicletas) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IdEstacionOrigen == IdEstacionDestino)
            yield return new ValidationResult("El origen y el destino deben ser distintos.", [nameof(IdEstacionDestino)]);
    }
}

// agregarBicicletaAlMovimiento() - idMovimiento va en la ruta
public record AgregarBicicletaAlMovimientoRequest(
    [property: Range(1, int.MaxValue)] int IdBicicleta);

// ConsultaHistorialMovimiento()
public record FiltroMovimientosRequest(
    DateTime? Desde,
    DateTime? Hasta,
    int? IdEstacion,
    [property: Range(1, int.MaxValue)] int Page = 1,
    [property: Range(1, 100)] int PageSize = 20);

#endregion

#region Responses

public record GetDetalleMovimientoResponse(
    int IdMovimiento,
    int IdBicicleta,
    string Serial,
    string Marca,
    string Modelo);

public record GetMovimientoResponse(
    int IdMovimiento,
    int IdUsuario,
    string NombreAdministrador,
    int IdEstacionOrigen,
    string NombreEstacionOrigen,
    int IdEstacionDestino,
    string NombreEstacionDestino,
    DateTime FechaHora,
    string Motivo,
    IReadOnlyList<GetDetalleMovimientoResponse> Bicicletas);

#endregion