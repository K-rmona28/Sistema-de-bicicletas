using ApiCycleOn.Data;
using ApiCycleOn.DTOs;
using ApiCycleOn.Enums;
using ApiCycleOn.Exceptions;
using ApiCycleOn.Mapeos;
using ApiCycleOn.Patterns.State;
using ApiCycleOn.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Facades;

public sealed class UsuarioFacade(CycleOnContext db, IPasswordHasher hasher)
{
    public async Task<GetUsuarioResponse> ObtenerAsync(int idUsuario, CancellationToken ct) =>
        await db.Usuarios.AsNoTracking()
            .Where(u => u.IdUsuario == idUsuario)
            .Select(UsuarioMapeo.Proyeccion)
            .FirstOrDefaultAsync(ct)
        ?? throw new NoEncontradoException("Usuario no encontrado.");

    // ActualizarDatos()
    public async Task<GetUsuarioResponse> ActualizarDatosAsync(int idUsuario, ActualizarDatosUsuarioRequest req, CancellationToken ct)
    {
        if (req.FechaDeNacimiento > DateOnly.FromDateTime(DateTime.Today))
            throw new ReglaNegocioException("La fecha de nacimiento no puede ser futura.");

        var usuario = await db.Usuarios.Include(u => u.Administrador)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, ct)
            ?? throw new NoEncontradoException("Usuario no encontrado.");

        usuario.Nombre = req.Nombre.Trim();
        usuario.Apellido = req.Apellido.Trim();
        usuario.Telefono = string.IsNullOrWhiteSpace(req.Telefono) ? null : req.Telefono.Trim();
        usuario.FechaDeNacimiento = req.FechaDeNacimiento;

        await db.SaveChangesAsync(ct);
        return UsuarioMapeo.ToResponse(usuario);
    }

    // EliminarCuenta(): eliminación lógica, para no romper el historial de préstamos, pagos y facturas
    public async Task EliminarCuentaAsync(int idUsuario, EliminarCuentaRequest req, CancellationToken ct)
    {
        var usuario = await db.Usuarios.Include(u => u.Administrador)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, ct)
            ?? throw new NoEncontradoException("Usuario no encontrado.");

        if (!hasher.Verify(req.Pass, usuario.Pass))
            throw new ReglaNegocioException("La contraseña no es correcta.");

        var tieneActivos =
            await db.Prestamos.AnyAsync(p => p.IdUsuario == idUsuario &&
                (p.Estado == EstadoPrestamoEnum.ACEPTADO || p.Estado == EstadoPrestamoEnum.EN_PROCESO || p.Estado == EstadoPrestamoEnum.VENCIDO), ct)
            || await db.Reservas.AnyAsync(r => r.IdUsuario == idUsuario && r.Estado == EstadoReservaEnum.VIGENTE, ct);

        if (tieneActivos)
            throw new ReglaNegocioException("No puedes eliminar tu cuenta mientras tengas préstamos o reservas activas.");

        if (usuario.Administrador is not null)
        {
            var otrosAdmins = await db.Administradors.CountAsync(a =>
                a.IdUsuario != idUsuario && a.IdUsuarioNavigation.Estado == EstadosUsuario.Activo, ct);

            if (otrosAdmins == 0)
                throw new ReglaNegocioException("Eres el único administrador activo; crea otro antes de eliminar tu cuenta.");
        }

        usuario.Estado = UsuarioStateFactory.De(usuario.Estado).Eliminar();
        await db.SaveChangesAsync(ct);
    }

    // FiltrarUsuarios()
    public async Task<GetPagedResponse<GetUsuarioResponse>> FiltrarAsync(FiltrarUsuariosRequest req, CancellationToken ct)
    {
        var consulta = db.Usuarios.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Texto))
        {
            var texto = req.Texto.Trim();
            consulta = consulta.Where(u => u.Nombre.Contains(texto) || u.Apellido.Contains(texto) || u.Email.Contains(texto));
        }

        if (!string.IsNullOrWhiteSpace(req.Estado))
        {
            var estado = req.Estado.Trim().ToUpperInvariant();
            consulta = consulta.Where(u => u.Estado == estado);
        }

        if (string.Equals(req.Rol, Roles.Administrador, StringComparison.OrdinalIgnoreCase))
            consulta = consulta.Where(u => u.Administrador != null);
        else if (string.Equals(req.Rol, Roles.Cliente, StringComparison.OrdinalIgnoreCase))
            consulta = consulta.Where(u => u.Cliente != null);

        var total = await consulta.CountAsync(ct);

        var items = await consulta
            .OrderBy(u => u.IdUsuario)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(UsuarioMapeo.Proyeccion)
            .ToListAsync(ct);

        return new GetPagedResponse<GetUsuarioResponse>(items, req.Page, req.PageSize, total);
    }
}