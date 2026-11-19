using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

public record RegistrarAdministradorRequest(
    [property: Required, StringLength(80)] string Nombre,
    [property: Required, StringLength(80)] string Apellido,
    [property: Required, EmailAddress, StringLength(150)] string Email,
    [property: Required, StringLength(100, MinimumLength = 8)] string Pass,
    [property: Phone, StringLength(20)] string? Telefono,
    DateOnly FechaDeNacimiento,
    [property: Required, StringLength(80)] string Cargo);

// FiltrarUsuarios()
public record FiltrarUsuariosRequest(
    [property: StringLength(100)] string? Texto,      // nombre, apellido o email
    [property: StringLength(20)] string? Estado,
    [property: StringLength(20)] string? Rol,         // Administrador | Cliente
    [property: Range(1, int.MaxValue)] int Page = 1,
    [property: Range(1, 100)] int PageSize = 20);

// MonitorearPrestamosActivos()
public record FiltroPrestamosActivosRequest(
    int? IdEstacion,
    int? IdUsuario,
    [property: Range(1, int.MaxValue)] int Page = 1,
    [property: Range(1, 100)] int PageSize = 20);

// GenerarReporteGeneral()
public record GenerarReporteGeneralRequest(
    DateTime Desde,
    DateTime Hasta);

// GenerarReportePorUsuario(idUsuario) - idUsuario va en la ruta
public record GenerarReportePorUsuarioRequest(
    DateTime Desde,
    DateTime Hasta);

// ConsultarHistorialUsuario(idUsuario) -> FiltroHistorialRequest (PrestamoDTOs), idUsuario en la ruta
// GestionarBicicletas() -> BicicletaDTOs | GestionarMantenimiento() -> MantenimientoDTOs
// ManejoDeReportes() -> ReporteDTOs     | AplicacionDeSanciones() -> SancionDTOs

#endregion

#region Responses

public record GetAdministradorResponse(
    GetUsuarioResponse Usuario,
    string Cargo);

// MonitorearBicicleta()
public record GetMonitoreoBicicletaResponse(
    int IdBicicleta,
    string Serial,
    string Estado,
    int? IdEstacion,
    string? NombreEstacion,
    int? IdPrestamoActivo,
    decimal? UltimaLatitud,
    decimal? UltimaLongitud,
    DateTime? UltimaActualizacion);

public record GetReporteGeneralResponse(
    DateTime Desde,
    DateTime Hasta,
    int TotalPrestamos,
    int PrestamosFinalizados,
    int PrestamosCancelados,
    decimal IngresosTotales,
    double TiempoPromedioUsoMinutos,
    int TotalSanciones,
    int ReportesAbiertos,
    int BicicletasEnMantenimiento);

public record GetReportePorUsuarioResponse(
    int IdUsuario,
    string NombreCompleto,
    DateTime Desde,
    DateTime Hasta,
    int TotalPrestamos,
    int TiempoTotalUsoMinutos,
    decimal GastoTotal,
    int TotalSanciones,
    int TotalReportes);

#endregion