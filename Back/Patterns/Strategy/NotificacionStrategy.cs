namespace ApiCycleOn.Patterns.Strategy;

// PATRÓN STRATEGY (2/2): canal de notificación (equivale a la <<interface>> Notificacion del diagrama)
public enum CanalNotificacion { Email, WhatsApp }

public sealed record DestinoNotificacion(int IdUsuario, string Nombre, string Email, string? Telefono);

public interface INotificacionStrategy
{
    CanalNotificacion Canal { get; }
    Task<bool> EnviarAsync(DestinoNotificacion destino, string asunto, string mensaje, CancellationToken ct);
}

// NOTA: envío SIMULADO (solo log). Cuando tengas proveedor (SMTP/MailKit, SendGrid...) se implementa aquí
// y el resto del sistema no cambia.
public sealed class EmailNotificacionStrategy(ILogger<EmailNotificacionStrategy> logger) : INotificacionStrategy
{
    public CanalNotificacion Canal => CanalNotificacion.Email;

    public Task<bool> EnviarAsync(DestinoNotificacion destino, string asunto, string mensaje, CancellationToken ct)
    {
        logger.LogInformation("[EMAIL simulado] Para: {Email} | Asunto: {Asunto}", destino.Email, asunto);
        return Task.FromResult(true);
    }
}

public sealed class WhatsAppNotificacionStrategy(ILogger<WhatsAppNotificacionStrategy> logger) : INotificacionStrategy
{
    public CanalNotificacion Canal => CanalNotificacion.WhatsApp;

    public Task<bool> EnviarAsync(DestinoNotificacion destino, string asunto, string mensaje, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(destino.Telefono))
        {
            logger.LogWarning("[WhatsApp] El usuario {Id} no tiene teléfono registrado.", destino.IdUsuario);
            return Task.FromResult(false);
        }

        logger.LogInformation("[WhatsApp simulado] Para: {Telefono} | {Asunto}", destino.Telefono, asunto);
        return Task.FromResult(true);
    }
}
