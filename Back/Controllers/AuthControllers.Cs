using ApiCycleOn.DTOs;
using ApiCycleOn.Enums;
using ApiCycleOn.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCycleOn.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthFacade facade) : ControllerBase
{
    // AutenticarUsuario()
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<GetLoginResponse>> Login(LoginRequest request, CancellationToken ct) =>
        Ok(await facade.LoginAsync(request, ct));

    // Registro público: siempre crea un Cliente
    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<GetClienteResponse>> Registro(RegistrarClienteRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await facade.RegistrarClienteAsync(request, ct));

    // Solo un administrador puede crear otros administradores
    [HttpPost("registro-administrador")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<GetAdministradorResponse>> RegistroAdministrador(
        RegistrarAdministradorRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await facade.RegistrarAdministradorAsync(request, ct));

    // RestablecerAcceso() paso 1: siempre responde igual (no revela si el correo existe)
    [HttpPost("restablecer-acceso/solicitar")]
    [AllowAnonymous]
    public async Task<ActionResult<GetMensajeResponse>> SolicitarRestablecimiento(
        SolicitarRestablecerAccesoRequest request, CancellationToken ct)
    {
        await facade.SolicitarRestablecimientoAsync(request, ct);
        return Ok(new GetMensajeResponse("Si el correo está registrado, recibirás un enlace para restablecer tu acceso."));
    }

    // RestablecerAcceso() paso 2
    [HttpPost("restablecer-acceso/confirmar")]
    [AllowAnonymous]
    public async Task<ActionResult<GetMensajeResponse>> ConfirmarRestablecimiento(
        RestablecerAccesoRequest request, CancellationToken ct)
    {
        await facade.RestablecerAccesoAsync(request, ct);
        return Ok(new GetMensajeResponse("Tu contraseña fue actualizada. Ya puedes iniciar sesión."));
    }
}
