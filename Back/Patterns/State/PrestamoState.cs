using ApiCycleOn.Enums;
using ApiCycleOn.Exceptions;

namespace ApiCycleOn.Patterns.State;

// ACEPTADO -> EN_PROCESO -> FINALIZADO
//     \___________\____-> CANCELADO | VENCIDO
public abstract class PrestamoState
{
    public abstract EstadoPrestamoEnum Estado { get; }
    public virtual bool EstaActivo => false;

    public virtual EstadoPrestamoEnum Iniciar() =>
        throw new TransicionInvalidaException("el préstamo", Estado.ToString(), "iniciar");

    public virtual EstadoPrestamoEnum Finalizar() =>
        throw new TransicionInvalidaException("el préstamo", Estado.ToString(), "finalizar");

    public virtual EstadoPrestamoEnum Cancelar() =>
        throw new TransicionInvalidaException("el préstamo", Estado.ToString(), "cancelar");

    public virtual EstadoPrestamoEnum Vencer() =>
        throw new TransicionInvalidaException("el préstamo", Estado.ToString(), "marcar como vencido");
}

public sealed class PrestamoAceptadoState : PrestamoState
{
    public override EstadoPrestamoEnum Estado => EstadoPrestamoEnum.ACEPTADO;
    public override bool EstaActivo => true;

    public override EstadoPrestamoEnum Iniciar() =>
        EstadoPrestamoEnum.EN_PROCESO;

    public override EstadoPrestamoEnum Cancelar() =>
        EstadoPrestamoEnum.CANCELADO;

    public override EstadoPrestamoEnum Vencer() =>
        EstadoPrestamoEnum.VENCIDO;
}

public sealed class PrestamoEnProcesoState : PrestamoState
{
    public override EstadoPrestamoEnum Estado => EstadoPrestamoEnum.EN_PROCESO;
    public override bool EstaActivo => true;

    public override EstadoPrestamoEnum Finalizar() =>
        EstadoPrestamoEnum.FINALIZADO;

    public override EstadoPrestamoEnum Cancelar() =>
        EstadoPrestamoEnum.CANCELADO;

    public override EstadoPrestamoEnum Vencer() =>
        EstadoPrestamoEnum.VENCIDO;
}

public sealed class PrestamoFinalizadoState : PrestamoState
{
    public override EstadoPrestamoEnum Estado => EstadoPrestamoEnum.FINALIZADO;
}

public sealed class PrestamoCanceladoState : PrestamoState
{
    public override EstadoPrestamoEnum Estado => EstadoPrestamoEnum.CANCELADO;
}

public sealed class PrestamoVencidoState : PrestamoState
{
    public override EstadoPrestamoEnum Estado => EstadoPrestamoEnum.VENCIDO;
    public override bool EstaActivo => true;

    public override EstadoPrestamoEnum Finalizar() =>
        EstadoPrestamoEnum.FINALIZADO;
}

public static class PrestamoStateFactory
{
    private static readonly PrestamoState Aceptado = new PrestamoAceptadoState();
    private static readonly PrestamoState EnProceso = new PrestamoEnProcesoState();
    private static readonly PrestamoState Finalizado = new PrestamoFinalizadoState();
    private static readonly PrestamoState Cancelado = new PrestamoCanceladoState();
    private static readonly PrestamoState Vencido = new PrestamoVencidoState();

    public static PrestamoState De(string? estado) =>
        Enum.TryParse<EstadoPrestamoEnum>(estado?.Trim(), true, out var e)
            ? De(e)
            : throw new ReglaNegocioException($"Estado de préstamo desconocido: {estado}");

    public static PrestamoState De(EstadoPrestamoEnum estado) => estado switch
    {
        EstadoPrestamoEnum.ACEPTADO => Aceptado,
        EstadoPrestamoEnum.EN_PROCESO => EnProceso,
        EstadoPrestamoEnum.FINALIZADO => Finalizado,
        EstadoPrestamoEnum.CANCELADO => Cancelado,
        EstadoPrestamoEnum.VENCIDO => Vencido,
        _ => throw new ReglaNegocioException($"Estado de préstamo desconocido: {estado}")
    };
}