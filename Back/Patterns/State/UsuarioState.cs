using ApiCycleOn.Exceptions;

namespace ApiCycleOn.Patterns.State;

// PATRÓN STATE: cada estado decide qué transiciones permite y a qué estado lleva.
// Los estados no guardan datos, así que se reutilizan como singletons (cero asignaciones por petición).
public abstract class UsuarioState
{
    public abstract string Nombre { get; }
    public virtual bool PuedeAutenticar => false;
    public virtual string MotivoRechazo => "La cuenta no está disponible.";

    public virtual string Bloquear() => throw new TransicionInvalidaException("al usuario", Nombre, "bloquear");
    public virtual string Desbloquear() => throw new TransicionInvalidaException("al usuario", Nombre, "desbloquear");
    public virtual string Eliminar() => throw new TransicionInvalidaException("al usuario", Nombre, "eliminar");
}

public sealed class UsuarioActivoState : UsuarioState
{
    public override string Nombre => EstadosUsuario.Activo;
    public override bool PuedeAutenticar => true;
    public override string Bloquear() => EstadosUsuario.Bloqueado;
    public override string Eliminar() => EstadosUsuario.Eliminado;
}

public sealed class UsuarioBloqueadoState : UsuarioState
{
    public override string Nombre => EstadosUsuario.Bloqueado;
    public override string MotivoRechazo => "Tu cuenta está bloqueada. Restablece tu acceso para recuperarla.";
    public override string Desbloquear() => EstadosUsuario.Activo;
    public override string Eliminar() => EstadosUsuario.Eliminado;
}

public sealed class UsuarioEliminadoState : UsuarioState
{
    public override string Nombre => EstadosUsuario.Eliminado;
    public override string MotivoRechazo => "Esta cuenta fue eliminada.";
}

public static class UsuarioStateFactory
{
    private static readonly UsuarioState Activo = new UsuarioActivoState();
    private static readonly UsuarioState Bloqueado = new UsuarioBloqueadoState();
    private static readonly UsuarioState Eliminado = new UsuarioEliminadoState();

    public static UsuarioState De(string? estado) => estado?.Trim().ToUpperInvariant() switch
    {
        EstadosUsuario.Activo => Activo,
        EstadosUsuario.Bloqueado => Bloqueado,
        EstadosUsuario.Eliminado => Eliminado,
        _ => throw new ReglaNegocioException($"Estado de usuario desconocido: {estado}")
    };
}
