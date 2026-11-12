using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GestionNotas.Api.Salud;

/// <summary>Escribe el resultado del health check como JSON legible.</summary>
public static class RespuestaSalud
{
    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };

    public static Task EscribirAsync(HttpContext context, HealthReport reporte)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var cuerpo = new
        {
            estado = reporte.Status.ToString(),
            duracionMs = Math.Round(reporte.TotalDuration.TotalMilliseconds, 1),
            verificaciones = reporte.Entries.Select(e => new
            {
                nombre = e.Key,
                estado = e.Value.Status.ToString(),
                descripcion = e.Value.Description,
                duracionMs = Math.Round(e.Value.Duration.TotalMilliseconds, 1)
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(cuerpo, Opciones));
    }
}
