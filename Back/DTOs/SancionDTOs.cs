using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// AplicarPenalizacion()
public record AplicarPenalizacionRequest(
    [property: Range(1, int.MaxValue)] int IdUsuario,
    int? IdPrestamo,
    int? IdReporte,
    [property: Required, StringLength(200)] string Motivo,
    [property: StringLength(500)] string? Descripcion,
    [property: Required, StringLength(50)] string TipoDePenalizacion);

// ModificarSancion() - idSancion va en la ruta
public record ModificarSancionRequest(
    [property: Required, StringLength(200)] string Motivo,
    [property: StringLength(500)] string? Descripcion,
    [property: Required, StringLength(50)] string TipoDePenalizacion);

// RevocarSancion()
public record RevocarSancionRequest(
    [property: Required, StringLength(250)] string Motivo);

#endregion

#region Responses

public record GetSancionResponse(
    int IdSancion,
    int IdUsuario,
    int? IdPrestamo,
    int? IdReporte,
    string Motivo,
    DateTime FechaHora,
    string Estado,
    string? Descripcion,
    string TipoDePenalizacion);

#endregion