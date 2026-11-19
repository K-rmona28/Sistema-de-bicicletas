using System.Linq.Expressions;
using ApiCycleOn.DTOs;
using ApiCycleOn.Enums;
using ApiCycleOn.Models;

namespace ApiCycleOn.Mapeos;

public static class UsuarioMapeo
{
    // Proyección para consultas (EF la traduce a SQL y trae solo las columnas necesarias)
    public static readonly Expression<Func<Usuario, GetUsuarioResponse>> Proyeccion = u =>
        new GetUsuarioResponse(
            u.IdUsuario, u.Nombre, u.Apellido, u.Email, u.Estado, u.Telefono,
            u.FechaDeNacimiento, u.Edad ?? 0, u.FechaDeRegistro,
            u.Administrador != null ? Roles.Administrador : Roles.Cliente);

    private static readonly Func<Usuario, GetUsuarioResponse> Compilada = Proyeccion.Compile();

    // Para entidades ya cargadas (requiere Administrador incluido para saber el rol)
    public static GetUsuarioResponse ToResponse(Usuario u) => Compilada(u);
}
