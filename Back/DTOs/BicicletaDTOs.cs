using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// GestionarBicicletas() - crear
public record CrearBicicletaRequest(
    int? IdEstacion,
    [property: Required, StringLength(50)] string Tipo,
    [property: Required, StringLength(20)] string Tamano,
    [property: StringLength(250)] string? Especificacion,
    [property: Required, StringLength(60)] string Marca,
    [property: Required, StringLength(60)] string Modelo,
    [property: Required, StringLength(60)] string Serial,
    [property: Range(0, 999999999)] decimal Valor);

// GestionarBicicletas() - actualizar (el Serial no se modifica)
public record ActualizarBicicletaRequest(
    [property: Required, StringLength(50)] string Tipo,
    [property: Required, StringLength(20)] string Tamano,
    [property: StringLength(250)] string? Especificacion,
    [property: Required, StringLength(60)] string Marca,
    [property: Required, StringLength(60)] string Modelo,
    [property: Range(0, 999999999)] decimal Valor);

// CambiarEstado() - idBicicleta va en la ruta
public record CambiarEstadoBicicletaRequest(
    [property: Required, StringLength(20)] string Estado,
    [property: StringLength(250)] string? Motivo);

// Bloquear()
public record BloquearBicicletaRequest(
    [property: Required, StringLength(250)] string Motivo);

// MarcarMantenimiento() usa RegistrarMantenimientoRequest (MantenimientoDTOs)

#endregion

#region Responses

public record GetBicicletaResponse(
    int IdBicicleta,
    int? IdEstacion,
    string? NombreEstacion,
    string Tipo,
    string Tamano,
    string? Especificacion,
    string Estado,
    string Marca,
    string Modelo,
    string Serial,
    decimal Valor);

#endregion