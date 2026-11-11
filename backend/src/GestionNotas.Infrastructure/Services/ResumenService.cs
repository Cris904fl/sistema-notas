using GestionNotas.Application.Dtos;
using GestionNotas.Application.Interfaces;
using GestionNotas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Infrastructure.Services;

/// <summary>
/// Datos del tablero. Usa objetos de SQL Server (procedimiento y vistas) para que los
/// cálculos se hagan en la base de datos y solo viajen los resultados.
/// </summary>
public class ResumenService(GestionNotasDbContext db) : IResumenService
{
    public async Task<ResumenDto> ObtenerAsync(int cantidadMejores = 5, CancellationToken ct = default)
    {
        // Un procedimiento almacenado no se puede componer con LINQ: se materializa directamente.
        var indicadores = (await db.Database
            .SqlQuery<IndicadoresDto>($"EXEC dbo.sp_ResumenGeneral")
            .ToListAsync(ct)).Single();

        // Las vistas sí se pueden componer: el filtro, el orden y el TOP se ejecutan en SQL Server.
        var mejores = await db.Database
            .SqlQuery<PromedioDto>($"SELECT IdEstudiante AS Id, Estudiante AS Nombre, CantidadNotas, Promedio FROM dbo.vw_PromedioEstudiante")
            .Where(p => p.Promedio != null)
            .OrderByDescending(p => p.Promedio).ThenBy(p => p.Nombre)
            .Take(cantidadMejores)
            .ToListAsync(ct);

        var porProfesor = await db.Database
            .SqlQuery<PromedioDto>($"SELECT IdProfesor AS Id, Profesor AS Nombre, CantidadNotas, Promedio FROM dbo.vw_PromedioProfesor")
            .OrderByDescending(p => p.CantidadNotas).ThenBy(p => p.Nombre)
            .ToListAsync(ct);

        return new ResumenDto(indicadores, mejores, porProfesor);
    }
}
