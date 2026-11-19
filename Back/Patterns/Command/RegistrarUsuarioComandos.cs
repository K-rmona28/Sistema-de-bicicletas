using ApiCycleOn.Data;
using ApiCycleOn.DTOs;
using ApiCycleOn.Exceptions;
using ApiCycleOn.Mapeos;
using ApiCycleOn.Models;
using ApiCycleOn.Patterns.State;
using ApiCycleOn.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Patterns.Command;

internal static class RegistroUsuarioHelper
{
    public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    public static async Task ValidarAsync(CycleOnContext db, string email, DateOnly nacimiento, CancellationToken ct)
    {
        if (nacimiento > DateOnly.FromDateTime(DateTime.Today))
            throw new ReglaNegocioException("La fecha de nacimiento no puede ser futura.");

        if (await db.Usuarios.AnyAsync(u => u.Email == email, ct))
            throw new ConflictoException("Ya existe una cuenta con ese correo.");
    }

    public static Usuario Crear(string nombre, string apellido, string email, string passHash, string? telefono, DateOnly nacimiento) =>
        new()
        {
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Email = email,
            Pass = passHash,
            Estado = EstadosUsuario.Activo,
            Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim(),
            FechaDeNacimiento = nacimiento
        };

    public static Task MarcarEliminadoAsync(CycleOnContext db, int idUsuario, CancellationToken ct) =>
        db.Usuarios.Where(u => u.IdUsuario == idUsuario)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Estado, EstadosUsuario.Eliminado), ct);
}

public sealed class RegistrarClienteComando(RegistrarClienteRequest req, CycleOnContext db, IPasswordHasher hasher)
    : IComandoReversible<GetClienteResponse>
{
    private int _idCreado;

    public string Nombre => nameof(RegistrarClienteComando);

    public async Task<GetClienteResponse> EjecutarAsync(CancellationToken ct)
    {
        var email = RegistroUsuarioHelper.NormalizarEmail(req.Email);
        await RegistroUsuarioHelper.ValidarAsync(db, email, req.FechaDeNacimiento, ct);

        var usuario = RegistroUsuarioHelper.Crear(
            req.Nombre, req.Apellido, email, hasher.Hash(req.Pass), req.Telefono, req.FechaDeNacimiento);

        usuario.Cliente = new Cliente
        {
            NombreContactoEmergencia = req.NombreContactoEmergencia.Trim(),
            TelContactoEmergencia = req.TelContactoEmergencia.Trim(),
            MaximoPrestamos = 1
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);   // necesitamos el IdUsuario generado
        _idCreado = usuario.IdUsuario;

        return new GetClienteResponse(
            UsuarioMapeo.ToResponse(usuario),
            usuario.Cliente.NombreContactoEmergencia,
            usuario.Cliente.TelContactoEmergencia,
            usuario.Cliente.MaximoPrestamos);
    }

    public Task DeshacerAsync(CancellationToken ct) =>
        _idCreado == 0 ? Task.CompletedTask : RegistroUsuarioHelper.MarcarEliminadoAsync(db, _idCreado, ct);
}

public sealed class RegistrarAdministradorComando(RegistrarAdministradorRequest req, CycleOnContext db, IPasswordHasher hasher)
    : IComandoReversible<GetAdministradorResponse>
{
    private int _idCreado;

    public string Nombre => nameof(RegistrarAdministradorComando);

    public async Task<GetAdministradorResponse> EjecutarAsync(CancellationToken ct)
    {
        var email = RegistroUsuarioHelper.NormalizarEmail(req.Email);
        await RegistroUsuarioHelper.ValidarAsync(db, email, req.FechaDeNacimiento, ct);

        var usuario = RegistroUsuarioHelper.Crear(
            req.Nombre, req.Apellido, email, hasher.Hash(req.Pass), req.Telefono, req.FechaDeNacimiento);

        usuario.Administrador = new Administrador { Cargo = req.Cargo.Trim() };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);
        _idCreado = usuario.IdUsuario;

        return new GetAdministradorResponse(UsuarioMapeo.ToResponse(usuario), usuario.Administrador.Cargo);
    }

    public Task DeshacerAsync(CancellationToken ct) =>
        _idCreado == 0 ? Task.CompletedTask : RegistroUsuarioHelper.MarcarEliminadoAsync(db, _idCreado, ct);
}
