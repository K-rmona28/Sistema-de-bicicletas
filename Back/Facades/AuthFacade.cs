using ApiCycleOn.Data;
using ApiCycleOn.DTOs;
using ApiCycleOn.Enums;
using ApiCycleOn.Exceptions;
using ApiCycleOn.Mapeos;
using ApiCycleOn.Models;
using ApiCycleOn.Patterns.Command;
using ApiCycleOn.Patterns.Observer;
using ApiCycleOn.Patterns.State;
using ApiCycleOn.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Facades;

// PATRÓN FACADE: una sola puerta para el flujo de autenticación.
// Coordina: base de datos, hash, JWT, estados del usuario, comandos y eventos.
public sealed class AuthFacade(
    CycleOnContext db,
    IPasswordHasher hasher,
    ITokenService tokens,
    InvocadorComandos invocador,
    IPublicadorEventos eventos)
{
    private const int MaxIntentosFallidos = 5;

    // AutenticarUsuario()
    public async Task<GetLoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var usuario = await db.Usuarios.Include(u => u.Administrador)
            .FirstOrDefaultAsync(u => u.Email == email, ct)
            ?? throw new CredencialesInvalidasException();

        var estado = UsuarioStateFactory.De(usuario.Estado);

        if (!hasher.Verify(req.Pass, usuario.Pass))
        {
            await RegistrarIntentoFallidoAsync(usuario, estado, ct);
            throw new CredencialesInvalidasException();
        }

        if (!estado.PuedeAutenticar)
            throw new AccesoDenegadoException(estado.MotivoRechazo);

        if (usuario.IntentosFallidos > 0)
        {
            usuario.IntentosFallidos = 0;
            await db.SaveChangesAsync(ct);
        }

        var respuesta = UsuarioMapeo.ToResponse(usuario);
        var (token, expiracion) = tokens.GenerarAcceso(
            usuario.IdUsuario, usuario.Email, $"{usuario.Nombre} {usuario.Apellido}", respuesta.Rol);

        return new GetLoginResponse(token, expiracion, respuesta);
    }

    public async Task<GetClienteResponse> RegistrarClienteAsync(RegistrarClienteRequest req, CancellationToken ct)
    {
        var cliente = await invocador.EjecutarAsync(new RegistrarClienteComando(req, db, hasher), ct);
        await eventos.PublicarAsync(
            new UsuarioRegistradoEvento(cliente.Usuario.IdUsuario, cliente.Usuario.Nombre, Roles.Cliente), ct);
        return cliente;
    }

    public async Task<GetAdministradorResponse> RegistrarAdministradorAsync(RegistrarAdministradorRequest req, CancellationToken ct)
    {
        var admin = await invocador.EjecutarAsync(new RegistrarAdministradorComando(req, db, hasher), ct);
        await eventos.PublicarAsync(
            new UsuarioRegistradoEvento(admin.Usuario.IdUsuario, admin.Usuario.Nombre, Roles.Administrador), ct);
        return admin;
    }

    // RestablecerAcceso() paso 1. No revela si el correo existe.
    public async Task SolicitarRestablecimientoAsync(SolicitarRestablecerAccesoRequest req, CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var usuario = await db.Usuarios.AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => new { u.IdUsuario, u.Pass, u.Estado })
            .FirstOrDefaultAsync(ct);

        if (usuario is null || usuario.Estado == EstadosUsuario.Eliminado) return;

        var token = tokens.GenerarTokenRestablecimiento(usuario.IdUsuario, usuario.Pass);
        await eventos.PublicarAsync(new RestablecimientoSolicitadoEvento(usuario.IdUsuario, token), ct);
    }

    // RestablecerAcceso() paso 2. También desbloquea la cuenta si estaba bloqueada por intentos.
    public async Task RestablecerAccesoAsync(RestablecerAccesoRequest req, CancellationToken ct)
    {
        const string invalido = "El enlace de restablecimiento no es válido o ya expiró.";

        var datos = await tokens.LeerTokenRestablecimientoAsync(req.Token) ?? throw new ReglaNegocioException(invalido);

        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == datos.IdUsuario, ct)
            ?? throw new ReglaNegocioException(invalido);

        if (usuario.Estado == EstadosUsuario.Eliminado || tokens.HuellaPass(usuario.Pass) != datos.Huella)
            throw new ReglaNegocioException(invalido);

        usuario.Pass = hasher.Hash(req.NuevaPass);
        usuario.IntentosFallidos = 0;

        if (usuario.Estado == EstadosUsuario.Bloqueado)
            usuario.Estado = UsuarioStateFactory.De(usuario.Estado).Desbloquear();

        await db.SaveChangesAsync(ct);
        await eventos.PublicarAsync(new AccesoRestablecidoEvento(usuario.IdUsuario), ct);
    }

    // RegistrarIntentoFallido() + BloquearPorIntentos() son internos (# en el diagrama): no se exponen
    private async Task RegistrarIntentoFallidoAsync(Usuario usuario, UsuarioState estado, CancellationToken ct)
    {
        if (!estado.PuedeAutenticar) return;   // bloqueados o eliminados no acumulan intentos

        usuario.IntentosFallidos++;
        var bloquear = usuario.IntentosFallidos >= MaxIntentosFallidos;
        if (bloquear) usuario.Estado = estado.Bloquear();

        await db.SaveChangesAsync(ct);

        if (bloquear)
            await eventos.PublicarAsync(
                new UsuarioBloqueadoEvento(usuario.IdUsuario, "Demasiados intentos fallidos de inicio de sesión."), ct);
    }
}
