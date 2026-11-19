using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// EnviarNotificacion() por correo
public record EnviarNotificacionEmailRequest(
    [property: Range(1, int.MaxValue)] int IdUsuario,
    [property: Required, StringLength(1000)] string Mensaje);

#endregion

#region Responses

public record GetNotificacionEmailResponse(
    int IdNotificacion,
    int IdUsuario,
    string Mensaje,
    DateTime FechaHora,
    string Estado);

#endregion