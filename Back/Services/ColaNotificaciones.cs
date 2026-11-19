using System.Threading.Channels;
using ApiCycleOn.Patterns.Strategy;

namespace ApiCycleOn.Services;

// MensajeAlmacenado: lo que se guarda en la tabla (así un token o enlace sensible nunca queda en la base)
public sealed record NotificacionPendiente(
    int IdUsuario, string Asunto, string Mensaje, CanalNotificacion Canal, string? MensajeAlmacenado = null);

public interface IColaNotificaciones
{
    ValueTask EncolarAsync(NotificacionPendiente notificacion, CancellationToken ct = default);
}

// Cola en memoria: la petición del usuario solo encola (microsegundos) y un worker envía en segundo plano.
public sealed class ColaNotificaciones : IColaNotificaciones
{
    private readonly Channel<NotificacionPendiente> _canal = Channel.CreateBounded<NotificacionPendiente>(
        new BoundedChannelOptions(1000) { SingleReader = true, FullMode = BoundedChannelFullMode.Wait });

    public ValueTask EncolarAsync(NotificacionPendiente notificacion, CancellationToken ct = default) =>
        _canal.Writer.WriteAsync(notificacion, ct);

    public ChannelReader<NotificacionPendiente> Lector => _canal.Reader;
}
