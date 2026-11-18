using ApiCycleOn.Data;
using ApiCycleOn.Enums;
using ApiCycleOn.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Common;

public static class EstacionStateHelper
{
    public static int ContarBicicletasEnEstacion(IEnumerable<Bicicletum> bicicletas) =>
        bicicletas.Count(b =>
            b.Estado != EstadoBicicletaEnum.EN_PRESTAMO &&
            b.Estado != EstadoBicicletaEnum.DADA_DE_BAJA);

    public static int ContarBicicletasDisponibles(IEnumerable<Bicicletum> bicicletas) =>
        bicicletas.Count(b => b.Estado == EstadoBicicletaEnum.DISPONIBLE);

    public static EstadoEstacionEnum CalcularEstado(Estacion estacion)
    {
        if (estacion.Estado == EstadoEstacionEnum.INACTIVA)
            return EstadoEstacionEnum.INACTIVA;

        var enEstacion = ContarBicicletasEnEstacion(estacion.Bicicleta);
        var disponibles = ContarBicicletasDisponibles(estacion.Bicicleta);

        if (estacion.Capacidad > 0 && enEstacion >= estacion.Capacidad)
            return EstadoEstacionEnum.SATURADA;

        if (disponibles == 0)
            return EstadoEstacionEnum.SIN_STOCK;

        return EstadoEstacionEnum.OPERATIVA;
    }

    public static async Task ActualizarEstadoEstacionAsync(CycleOnContext context, int? idEstacion)
    {
        if (!idEstacion.HasValue) return;

        var estacion = await context.Estacions
            .Include(e => e.Bicicleta)
            .FirstOrDefaultAsync(e => e.IdEstacion == idEstacion.Value);

        if (estacion is null) return;

        var nuevoEstado = CalcularEstado(estacion);
        if (estacion.Estado != nuevoEstado)
        {
            estacion.Estado = nuevoEstado;
            await context.SaveChangesAsync();
        }
    }

    public static async Task ActualizarTodasLasEstacionesAsync(CycleOnContext context)
    {
        var estaciones = await context.Estacions
            .Include(e => e.Bicicleta)
            .ToListAsync();

        var huboCambios = false;
        foreach (var est in estaciones)
        {
            var nuevoEstado = CalcularEstado(est);
            if (est.Estado != nuevoEstado)
            {
                est.Estado = nuevoEstado;
                huboCambios = true;
            }
        }

        if (huboCambios)
            await context.SaveChangesAsync();
    }
}