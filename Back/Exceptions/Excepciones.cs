using Microsoft.AspNetCore.Http;

namespace ApiCycleOn.Exceptions;

// Excepciones de negocio: el ManejadorExcepciones las convierte en la respuesta HTTP correcta.
public abstract class AppException(string mensaje, int statusCode) : Exception(mensaje)
{
    public int StatusCode { get; } = statusCode;
}

public class NoEncontradoException(string mensaje)
    : AppException(mensaje, StatusCodes.Status404NotFound);

public class ReglaNegocioException(string mensaje)
    : AppException(mensaje, StatusCodes.Status400BadRequest);

public class ConflictoException(string mensaje)
    : AppException(mensaje, StatusCodes.Status409Conflict);

public class CredencialesInvalidasException(string mensaje = "Credenciales inválidas.")
    : AppException(mensaje, StatusCodes.Status401Unauthorized);

public class AccesoDenegadoException(string mensaje)
    : AppException(mensaje, StatusCodes.Status403Forbidden);

// La lanzan los objetos State cuando una transición no está permitida
public class TransicionInvalidaException(string entidad, string estadoActual, string accion)
    : ConflictoException($"No se puede {accion} {entidad} en estado {estadoActual}.");
