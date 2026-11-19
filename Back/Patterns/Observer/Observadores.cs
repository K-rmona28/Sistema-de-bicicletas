using ApiCycleOn.Patterns.Strategy;
using ApiCycleOn.Services;

namespace ApiCycleOn.Patterns.Observer;

// Se registran solos por reflexión (ver ServiceCollectionExtensions). Para reaccionar a un evento nuevo
// basta con crear otra clase que implemente IObservador<TEvento>.

public sealed class BienvenidaObserver(IColaNotificaciones cola) : IObservador<UsuarioRegistradoEvento>
{
    public async Task ManejarAsync(UsuarioRegistradoEvento e, CancellationToken ct) =>
        await cola.EncolarAsync(new NotificacionPendiente(
            e.IdUsuario, "Bienvenido a CycleOn",
            $"Hola {e.Nombre}, tu cuenta en CycleOn fue creada correctamente. ¡Ya puedes reservar tu bicicleta!",
            CanalNotificacion.Email), ct);
}

public sealed class RestablecimientoObserver(IColaNotificaciones cola, IConfiguration config)
    : IObservador<RestablecimientoSolicitadoEvento>
{
    public async Task ManejarAsync(RestablecimientoSolicitadoEvento e, CancellationToken ct)
    {
        var baseUrl = config["Frontend:UrlRestablecer"] ?? "http://localhost:4200/restablecer-acceso";
        var enlace = $"{baseUrl}?token={Uri.EscapeDataString(e.Token)}";

        await cola.EncolarAsync(new NotificacionPendiente(
            e.IdUsuario, "Restablece tu acceso a CycleOn",
            $"Usa este enlace para crear una nueva contraseña (válido por 30 minutos): {enlace}",
            CanalNotificacion.Email,
            MensajeAlmacenado: "Solicitud de restablecimiento de acceso."), ct);   // el enlace no se guarda en BD
    }
}

public sealed class AccesoRestablecidoObserver(IColaNotificaciones cola) : IObservador<AccesoRestablecidoEvento>
{
    public async Task ManejarAsync(AccesoRestablecidoEvento e, CancellationToken ct) =>
        await cola.EncolarAsync(new NotificacionPendiente(
            e.IdUsuario, "Tu contraseña fue cambiada",
            "Tu contraseña de CycleOn fue restablecida. Si no fuiste tú, contacta al administrador de inmediato.",
            CanalNotificacion.Email), ct);
}

public sealed class UsuarioBloqueadoObserver(IColaNotificaciones cola) : IObservador<UsuarioBloqueadoEvento>
{
    public async Task ManejarAsync(UsuarioBloqueadoEvento e, CancellationToken ct)
    {
        const string asunto = "Tu cuenta fue bloqueada";
        var mensaje = $"Tu cuenta de CycleOn fue bloqueada. Motivo: {e.Motivo}. Restablece tu acceso para recuperarla.";

        await cola.EncolarAsync(new NotificacionPendiente(e.IdUsuario, asunto, mensaje, CanalNotificacion.Email), ct);
        await cola.EncolarAsync(new NotificacionPendiente(e.IdUsuario, asunto, mensaje, CanalNotificacion.WhatsApp), ct);
    }
}
