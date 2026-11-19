namespace ApiCycleOn.Enums;

// Se usan en el claim de rol del JWT y en [Authorize(Roles = Roles.Administrador)]
public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Cliente = "Cliente";
}