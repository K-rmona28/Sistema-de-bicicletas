using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Exceptions;

// Un solo lugar para traducir excepciones a HTTP: los controllers quedan sin try/catch.
public sealed class ManejadorExcepciones(ILogger<ManejadorExcepciones> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception ex, CancellationToken ct)
    {
        var (status, titulo) = ex switch
        {
            AppException a => (a.StatusCode, a.Message),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict,
                "El registro fue modificado por otra persona. Intenta de nuevo."),
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } => (StatusCodes.Status409Conflict,
                "La operación entra en conflicto con un registro existente (dato duplicado o recurso ya en uso)."),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.")
        };

        if (status >= 500) logger.LogError(ex, "Error no controlado");
        else logger.LogWarning("{Tipo}: {Mensaje}", ex.GetType().Name, ex.Message);

        contexto.Response.StatusCode = status;
        await contexto.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = titulo }, ct);
        return true;
    }
}