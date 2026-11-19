using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

public record CrearEstacionRequest(
    [property: Required, StringLength(100)] string Nombre,
    [property: Required, StringLength(200)] string Ubicacion,
    [property: Range(1, 1000)] int Capacidad);

public record ActualizarEstacionRequest(
    [property: Required, StringLength(100)] string Nombre,
    [property: Required, StringLength(200)] string Ubicacion,
    [property: Range(1, 1000)] int Capacidad,
    [property: Required, StringLength(20)] string Estado);

// AgregarBicicleta() - idEstacion va en la ruta
public record AgregarBicicletaEstacionRequest(
    [property: Range(1, int.MaxValue)] int IdBicicleta);

// RetirarBicicleta()
public record RetirarBicicletaEstacionRequest(
    [property: Range(1, int.MaxValue)] int IdBicicleta,
    [property: StringLength(250)] string? Motivo);

// RedistribuirBicicleta() - la estación de la ruta es el origen; genera un Movimiento
public record RedistribuirBicicletaRequest(
    [property: Range(1, int.MaxValue)] int IdEstacionDestino,
    [property: Required, MinLength(1)] List<int> IdsBicicletas,
    [property: Required, StringLength(250)] string Motivo);

// VerInformacion() y VerBicicletasActivas() no necesitan request (ruta)

#endregion

#region Responses

public record GetEstacionResponse(
    int IdEstacion,
    string Nombre,
    string Ubicacion,
    int Capacidad,
    string Estado,
    int BicicletasActuales,
    int BicicletasDisponibles);

// VerBicicletasActivas() devuelve List<GetBicicletaResponse>

#endregion