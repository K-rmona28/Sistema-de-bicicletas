using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// RegistrarPago() - el monto lo toma el servidor del CostoTotal del préstamo, no del cliente
public record RegistrarPagoRequest(
    [property: Range(1, int.MaxValue)] int IdPrestamo,
    [property: Required, StringLength(30)] string MetodoPago);

// CancelarPago() - idPago va en la ruta
public record CancelarPagoRequest(
    [property: StringLength(250)] string? Motivo);

#endregion

#region Responses

public record GetPagoResponse(
    int IdPago,
    int IdPrestamo,
    string MetodoPago,
    decimal Monto,
    DateTime FechaHoraPago,
    string Estado);

#endregion