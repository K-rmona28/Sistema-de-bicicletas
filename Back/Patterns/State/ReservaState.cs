using ApiCycleOn.Enums;
using ApiCycleOn.Exceptions;

namespace ApiCycleOn.Patterns.State;

// VIGENTE -> UTILIZADA | CANCELADA | EXPIRADA
public abstract class ReservaState
{
    public abstract EstadoReservaEnum Estado { get; }
    public virtual bool EstaActiva => false;

    public virtual EstadoReservaEnum Utilizar() =>
        throw new TransicionInvalidaException("la reserva", Estado.ToString(), "utilizar");

    public virtual EstadoReservaEnum Cancelar() =>
        throw new TransicionInvalidaException("la reserva", Estado.ToString(), "cancelar");

    public virtual EstadoReservaEnum Expirar() =>
        throw new TransicionInvalidaException("la reserva", Estado.ToString(), "expirar");
}

public sealed class ReservaVigenteState : ReservaState
{
    public override EstadoReservaEnum Estado => EstadoReservaEnum.VIGENTE;
    public override bool EstaActiva => true;

    public override EstadoReservaEnum Utilizar() =>
        EstadoReservaEnum.UTILIZADA;

    public override EstadoReservaEnum Cancelar() =>
        EstadoReservaEnum.CANCELADA;

    public override EstadoReservaEnum Expirar() =>
        EstadoReservaEnum.EXPIRADA;
}

public sealed class ReservaUtilizadaState : ReservaState
{
    public override EstadoReservaEnum Estado => EstadoReservaEnum.UTILIZADA;
}

public sealed class ReservaCanceladaState : ReservaState
{
    public override EstadoReservaEnum Estado => EstadoReservaEnum.CANCELADA;
}

public sealed class ReservaExpiradaState : ReservaState
{
    public override EstadoReservaEnum Estado => EstadoReservaEnum.EXPIRADA;
}

public static class ReservaStateFactory
{
    private static readonly ReservaState Vigente = new ReservaVigenteState();
    private static readonly ReservaState Utilizada = new ReservaUtilizadaState();
    private static readonly ReservaState Cancelada = new ReservaCanceladaState();
    private static readonly ReservaState Expirada = new ReservaExpiradaState();

    public static ReservaState De(string? estado) =>
        Enum.TryParse<EstadoReservaEnum>(estado?.Trim(), true, out var e)
            ? De(e)
            : throw new ReglaNegocioException($"Estado de reserva desconocido: {estado}");

    public static ReservaState De(EstadoReservaEnum estado) => estado switch
    {
        EstadoReservaEnum.VIGENTE => Vigente,
        EstadoReservaEnum.UTILIZADA => Utilizada,
        EstadoReservaEnum.CANCELADA => Cancelada,
        EstadoReservaEnum.EXPIRADA => Expirada,
        _ => throw new ReglaNegocioException($"Estado de reserva desconocido: {estado}")
    };
}