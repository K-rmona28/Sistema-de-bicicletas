using System.Diagnostics;
using ApiCycleOn.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Patterns.Command;

// PATRÓN COMMAND: cada operación crítica es un objeto con su propia lógica.
// El invocador la ejecuta dentro de UNA transacción (todo o nada) y mide el tiempo.
public interface IComando<TResultado>
{
    string Nombre { get; }

    // Si el comando necesita IDs generados, puede llamar a SaveChangesAsync dentro (sigue en la transacción)
    Task<TResultado> EjecutarAsync(CancellationToken ct);
}

// Undo = compensación lógica (cancelar, revocar, devolver a su estado anterior), no un rollback de BD
public interface IComandoReversible<TResultado> : IComando<TResultado>
{
    Task DeshacerAsync(CancellationToken ct);
}

public sealed class InvocadorComandos(CycleOnContext db, ILogger<InvocadorComandos> logger)
{
    public async Task<TResultado> EjecutarAsync<TResultado>(IComando<TResultado> comando, CancellationToken ct = default)
    {
        var reloj = Stopwatch.StartNew();
        var estrategia = db.Database.CreateExecutionStrategy();

        var resultado = await estrategia.ExecuteAsync(async () =>
        {
            await using var transaccion = await db.Database.BeginTransactionAsync(ct);   // sin Commit => rollback
            var r = await comando.EjecutarAsync(ct);
            await db.SaveChangesAsync(ct);                                                // un solo guardado final
            await transaccion.CommitAsync(ct);
            return r;
        });

        logger.LogInformation("Comando {Comando} ejecutado en {Ms} ms", comando.Nombre, reloj.ElapsedMilliseconds);
        return resultado;
    }

    public async Task DeshacerAsync<TResultado>(IComandoReversible<TResultado> comando, CancellationToken ct = default)
    {
        await using var transaccion = await db.Database.BeginTransactionAsync(ct);
        await comando.DeshacerAsync(ct);
        await db.SaveChangesAsync(ct);
        await transaccion.CommitAsync(ct);

        logger.LogInformation("Comando {Comando} deshecho", comando.Nombre);
    }
}
