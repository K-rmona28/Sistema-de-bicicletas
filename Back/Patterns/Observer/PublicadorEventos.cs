namespace ApiCycleOn.Patterns.Observer;

// PATRÓN OBSERVER
public interface IObservador<in TEvento> where TEvento : IEventoDominio
{
    Task ManejarAsync(TEvento evento, CancellationToken ct);
}

public interface IPublicadorEventos
{
    Task PublicarAsync<TEvento>(TEvento evento, CancellationToken ct = default) where TEvento : IEventoDominio;
}

public sealed class PublicadorEventos(IServiceProvider servicios, ILogger<PublicadorEventos> logger) : IPublicadorEventos
{
    public async Task PublicarAsync<TEvento>(TEvento evento, CancellationToken ct = default) where TEvento : IEventoDominio
    {
        var observadores = servicios.GetServices<IObservador<TEvento>>().ToList();
        if (observadores.Count == 0) return;

        // Un observador que falla NO debe romper la operación principal ni a los demás observadores
        async Task Ejecutar(IObservador<TEvento> observador)
        {
            try { await observador.ManejarAsync(evento, ct); }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "El observador {Observador} falló con el evento {Evento}",
                    observador.GetType().Name, typeof(TEvento).Name);
            }
        }

        await Task.WhenAll(observadores.Select(Ejecutar));
    }
}
