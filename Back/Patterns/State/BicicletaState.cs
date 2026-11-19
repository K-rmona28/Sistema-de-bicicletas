using ApiCycleOn.Enums;
using ApiCycleOn.Exceptions;

namespace ApiCycleOn.Patterns.State;

// DISPONIBLE <-> RESERVADA -> EN_PRESTAMO -> DISPONIBLE
// DISPONIBLE -> EN_MANTENIMIENTO -> DISPONIBLE
// DISPONIBLE / RESERVADA / EN_MANTENIMIENTO -> DADA_DE_BAJA (-> DISPONIBLE al reactivar)
public abstract class BicicletaState
{
    public abstract EstadoBicicletaEnum Estado { get; }
    public virtual bool EstaDisponible => false;

    public virtual EstadoBicicletaEnum Reservar() => No("reservar");
    public virtual EstadoBicicletaEnum LiberarReserva() => No("liberar la reserva de");
    public virtual EstadoBicicletaEnum IniciarUso() => No("iniciar el uso de");
    public virtual EstadoBicicletaEnum Devolver() => No("devolver");
    public virtual EstadoBicicletaEnum EnviarAMantenimiento() => No("enviar a mantenimiento");
    public virtual EstadoBicicletaEnum FinalizarMantenimiento() => No("finalizar el mantenimiento de");
    public virtual EstadoBicicletaEnum DarDeBaja() => No("dar de baja");
    public virtual EstadoBicicletaEnum Reactivar() => No("reactivar");

    // Alias por compatibilidad si algún controlador llama Bloquear() / Desbloquear()
    public virtual EstadoBicicletaEnum Bloquear() => DarDeBaja();
    public virtual EstadoBicicletaEnum Desbloquear() => Reactivar();

    private EstadoBicicletaEnum No(string accion) =>
        throw new TransicionInvalidaException("la bicicleta", Estado.ToString(), accion);
}

public sealed class BicicletaDisponibleState : BicicletaState
{
    public override EstadoBicicletaEnum Estado => EstadoBicicletaEnum.DISPONIBLE;
    public override bool EstaDisponible => true;

    public override EstadoBicicletaEnum Reservar() =>
        EstadoBicicletaEnum.RESERVADA;

    public override EstadoBicicletaEnum IniciarUso() =>
        EstadoBicicletaEnum.EN_PRESTAMO;

    public override EstadoBicicletaEnum EnviarAMantenimiento() =>
        EstadoBicicletaEnum.EN_MANTENIMIENTO;

    public override EstadoBicicletaEnum DarDeBaja() =>
        EstadoBicicletaEnum.DADA_DE_BAJA;
}

public sealed class BicicletaReservadaState : BicicletaState
{
    public override EstadoBicicletaEnum Estado => EstadoBicicletaEnum.RESERVADA;

    public override EstadoBicicletaEnum LiberarReserva() =>
        EstadoBicicletaEnum.DISPONIBLE;

    public override EstadoBicicletaEnum IniciarUso() =>
        EstadoBicicletaEnum.EN_PRESTAMO;

    public override EstadoBicicletaEnum EnviarAMantenimiento() =>
        EstadoBicicletaEnum.EN_MANTENIMIENTO;

    public override EstadoBicicletaEnum DarDeBaja() =>
        EstadoBicicletaEnum.DADA_DE_BAJA;
}

public sealed class BicicletaEnPrestamoState : BicicletaState
{
    public override EstadoBicicletaEnum Estado => EstadoBicicletaEnum.EN_PRESTAMO;

    public override EstadoBicicletaEnum Devolver() =>
        EstadoBicicletaEnum.DISPONIBLE;
}

public sealed class BicicletaMantenimientoState : BicicletaState
{
    public override EstadoBicicletaEnum Estado => EstadoBicicletaEnum.EN_MANTENIMIENTO;

    public override EstadoBicicletaEnum FinalizarMantenimiento() =>
        EstadoBicicletaEnum.DISPONIBLE;

    public override EstadoBicicletaEnum DarDeBaja() =>
        EstadoBicicletaEnum.DADA_DE_BAJA;
}

public sealed class BicicletaDadaDeBajaState : BicicletaState
{
    public override EstadoBicicletaEnum Estado => EstadoBicicletaEnum.DADA_DE_BAJA;

    public override EstadoBicicletaEnum Reactivar() =>
        EstadoBicicletaEnum.DISPONIBLE;

    public override EstadoBicicletaEnum EnviarAMantenimiento() =>
        EstadoBicicletaEnum.EN_MANTENIMIENTO;
}

public static class BicicletaStateFactory
{
    private static readonly BicicletaState Disponible = new BicicletaDisponibleState();
    private static readonly BicicletaState Reservada = new BicicletaReservadaState();
    private static readonly BicicletaState EnPrestamo = new BicicletaEnPrestamoState();
    private static readonly BicicletaState EnMantenimiento = new BicicletaMantenimientoState();
    private static readonly BicicletaState DadaDeBaja = new BicicletaDadaDeBajaState();

    public static BicicletaState De(string? estado) =>
        Enum.TryParse<EstadoBicicletaEnum>(estado?.Trim(), true, out var e)
            ? De(e)
            : throw new ReglaNegocioException($"Estado de bicicleta desconocido: {estado}");

    public static BicicletaState De(EstadoBicicletaEnum estado) => estado switch
    {
        EstadoBicicletaEnum.DISPONIBLE => Disponible,
        EstadoBicicletaEnum.RESERVADA => Reservada,
        EstadoBicicletaEnum.EN_PRESTAMO => EnPrestamo,
        EstadoBicicletaEnum.EN_MANTENIMIENTO => EnMantenimiento,
        EstadoBicicletaEnum.DADA_DE_BAJA => DadaDeBaja,
        _ => throw new ReglaNegocioException($"Estado de bicicleta desconocido: {estado}")
    };
}