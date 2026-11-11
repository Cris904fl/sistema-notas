using GestionNotas.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GestionNotas.Api.Middleware;

/// <summary>
/// Convierte las excepciones en respuestas uniformes application/problem+json (RFC 9457).
/// Los errores no previstos se registran y salen como 500 sin exponer detalles internos.
/// </summary>
public class ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, titulo) = ex switch
            {
                NoEncontradoException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ConflictoException    => (StatusCodes.Status409Conflict, "Conflicto con el estado actual"),
                ValidacionException   => (StatusCodes.Status400BadRequest, "Datos inválidos"),
                _                     => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}", context.Request.Method, context.Request.Path);

            var problema = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado. Intente de nuevo más tarde."
                    : ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problema, options: null, contentType: "application/problem+json");
        }
    }
}
