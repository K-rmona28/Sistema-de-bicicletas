using ApiCycleOn.Common;
using ApiCycleOn.DTOs;
using ApiCycleOn.Enums;
using ApiCycleOn.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCycleOn.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController(UsuarioFacade facade) : ControllerBase
{
    // Perfil del usuario autenticado
    [HttpGet("me")]
    public async Task<ActionResult<GetUsuarioResponse>> ObtenerMiPerfil(CancellationToken ct) =>
        Ok(await facade.ObtenerAsync(User.GetUserId(), ct));

    // ActualizarDatos()
    [HttpPut("me")]
    public async Task<ActionResult<GetUsuarioResponse>> ActualizarMisDatos(
        ActualizarDatosUsuarioRequest request, CancellationToken ct) =>
        Ok(await facade.ActualizarDatosAsync(User.GetUserId(), request, ct));

    // EliminarCuenta() - en Angular: http.delete(url, { body: { pass } })
    [HttpDelete("me")]
    public async Task<IActionResult> EliminarMiCuenta(EliminarCuentaRequest request, CancellationToken ct)
    {
        await facade.EliminarCuentaAsync(User.GetUserId(), request, ct);
        return NoContent();
    }

    // FiltrarUsuarios() - administrador
    [HttpGet]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<GetPagedResponse<GetUsuarioResponse>>> Filtrar(
        [FromQuery] FiltrarUsuariosRequest request, CancellationToken ct) =>
        Ok(await facade.FiltrarAsync(request, ct));

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<GetUsuarioResponse>> ObtenerPorId(int id, CancellationToken ct) =>
        Ok(await facade.ObtenerAsync(id, ct));
}
