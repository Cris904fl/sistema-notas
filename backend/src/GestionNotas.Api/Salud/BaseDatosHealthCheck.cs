using GestionNotas.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GestionNotas.Api.Salud;

/// <summary>Verifica que la API pueda conectarse a SQL Server.</summary>
public class BaseDatosHealthCheck(GestionNotasDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            return await db.Database.CanConnectAsync(ct)
                ? HealthCheckResult.Healthy("Conexión con SQL Server correcta.")
                : HealthCheckResult.Unhealthy("No fue posible conectarse a SQL Server.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error al conectarse a SQL Server.", ex);
        }
    }
}
