using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// AutenticarUsuario()
public record LoginRequest(
    [property: Required, EmailAddress, StringLength(150)] string Email,
    [property: Required, StringLength(100)] string Pass);

// RestablecerAcceso() - paso 1: pedir el restablecimiento
public record SolicitarRestablecerAccesoRequest(
    [property: Required, EmailAddress, StringLength(150)] string Email);

// RestablecerAcceso() - paso 2: el token llega por correo / WhatsApp
public record RestablecerAccesoRequest(
    [property: Required] string Token,
    [property: Required, StringLength(100, MinimumLength = 8)] string NuevaPass);

// ActualizarDatos()
public record ActualizarDatosUsuarioRequest(
    [property: Required, StringLength(80)] string Nombre,
    [property: Required, StringLength(80)] string Apellido,
    [property: Phone, StringLength(20)] string? Telefono,
    DateOnly FechaDeNacimiento);

// EliminarCuenta() - confirma con la contraseña
public record EliminarCuentaRequest(
    [property: Required] string Pass);

#endregion

#region Responses

// Nunca incluye Pass ni IntentosFallidos
public record GetUsuarioResponse(
    int IdUsuario,
    string Nombre,
    string Apellido,
    string Email,
    string Estado,
    string? Telefono,
    DateOnly FechaDeNacimiento,
    int Edad,
    DateTime FechaDeRegistro,
    string Rol);

public record GetLoginResponse(
    string Token,
    DateTime Expiracion,
    GetUsuarioResponse Usuario);

#endregion