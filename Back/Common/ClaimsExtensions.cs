using System.Security.Claims;
using ApiCycleOn.Enums;
using ApiCycleOn.Exceptions;

namespace ApiCycleOn.Common;

public static class ClaimsExtensions
{
    public const string ClaimRol = "role";

    // El id del usuario autenticado SIEMPRE sale del token, nunca del body
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : throw new AccesoDenegadoException("Token inválido.");
    }

    public static string GetRol(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimRol) ?? user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public static bool EsAdministrador(this ClaimsPrincipal user) => user.GetRol() == Roles.Administrador;
}
