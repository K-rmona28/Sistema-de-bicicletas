using ApiCycleOn.Data;
using ApiCycleOn.Models;
using ApiCycleOn.Patterns.State;
using ApiCycleOn.Patterns.Strategy;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Services;

public sealed class NotificacionWorker(
    ColaNotificaciones cola,
    IServiceScopeFactory scopes,
    IEnumerable<INotificacionStrategy> estrategias,
    ILogger<NotificacionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mapa = estrategias.ToDictionary(e => e.Canal);

        await foreach (var notificacion in cola.Lector.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcesarAsync(notificacion, mapa, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "No se pudo procesar la notificación para el usuario {Id}", notificacion.IdUsuario);
            }
        }
    }

    private async Task ProcesarAsync(
        NotificacionPendiente n, Dictionary<CanalNotificacion, INotificacionStrategy> mapa, CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CycleOnContext>();

        var usuario = await db.Usuarios.AsNoTracking()
            .Where(u => u.IdUsuario == n.IdUsuario)
            .Select(u => new { u.Nombre, u.Apellido, u.Email, u.Telefono })
            .FirstOrDefaultAsync(ct);

        if (usuario is null) return;

        var destino = new DestinoNotificacion(n.IdUsuario, $"{usuario.Nombre} {usuario.Apellido}", usuario.Email, usuario.Telefono);

        bool enviada;
        try
        {
            enviada = await mapa[n.Canal].EnviarAsync(destino, n.Asunto, n.Mensaje, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Falló el envío por {Canal} al usuario {Id}", n.Canal, n.IdUsuario);
            enviada = false;
        }

        var estado = enviada ? EstadosNotificacion.Enviada : EstadosNotificacion.Fallida;
        var texto = Truncar(n.MensajeAlmacenado ?? n.Mensaje, 1000);

        // Un solo INSERT con el resultado final
        if (n.Canal == CanalNotificacion.Email)
            db.NotificacionEmails.Add(new NotificacionEmail { IdUsuario = n.IdUsuario, Mensaje = texto, Estado = estado });
        else
            db.NotificacionWhatsApps.Add(new NotificacionWhatsApp { IdUsuario = n.IdUsuario, Mensaje = texto, Estado = estado });

        await db.SaveChangesAsync(ct);
    }

    private static string Truncar(string texto, int max) => texto.Length <= max ? texto : texto[..max];
}
