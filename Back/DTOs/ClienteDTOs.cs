using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

public record RegistrarClienteRequest(
    [property: Required, StringLength(80)] string Nombre,
    [property: Required, StringLength(80)] string Apellido,
    [property: Required, EmailAddress, StringLength(150)] string Email,
    [property: Required, StringLength(100, MinimumLength = 8)] string Pass,
    [property: Phone, StringLength(20)] string? Telefono,
    DateOnly FechaDeNacimiento,
    [property: Required, StringLength(120)] string NombreContactoEmergencia,
    [property: Required, Phone, StringLength(20)] string TelContactoEmergencia);

public record ActualizarContactoEmergenciaRequest(
    [property: Required, StringLength(120)] string NombreContactoEmergencia,
    [property: Required, Phone, StringLength(20)] string TelContactoEmergencia);

// ConsultarDisponibilidad()
public record ConsultarDisponibilidadRequest(
    int? IdEstacion,
    [property: StringLength(50)] string? Tipo);

// VerHistorial() usa FiltroHistorialRequest (PrestamoDTOs)
// ConsultarCostoEstimado() usa CalcularValorServicioRequest (TarifaDTOs)
// SolicitarReporte() usa RegistrarReporteRequest (ReporteDTOs)

#endregion

#region Responses

public record GetClienteResponse(
    GetUsuarioResponse Usuario,
    string NombreContactoEmergencia,
    string TelContactoEmergencia,
    int MaximoPrestamos);

public record GetDisponibilidadEstacionResponse(
    int IdEstacion,
    string Nombre,
    string Ubicacion,
    int Capacidad,
    int BicicletasDisponibles);

#endregion