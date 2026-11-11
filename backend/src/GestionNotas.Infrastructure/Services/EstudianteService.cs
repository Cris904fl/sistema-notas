using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Exceptions;
using GestionNotas.Application.Interfaces;
using GestionNotas.Domain.Entities;
using GestionNotas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Infrastructure.Services;

public class EstudianteService(GestionNotasDbContext db) : IEstudianteService
{
    public async Task<ResultadoPaginado<EstudianteDto>> ListarAsync(PaginacionQuery query, CancellationToken ct = default)
    {
        var consulta = db.Estudiantes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Buscar))
            consulta = consulta.Where(e => e.Nombre.Contains(query.Buscar.Trim()));

        // El orden se aplica sobre la entidad (antes de proyectar) para que EF Core lo traduzca a SQL.
        consulta = (query.OrdenarPor?.ToLowerInvariant(), query.Descendente) switch
        {
            ("nombre", false)   => consulta.OrderBy(e => e.Nombre),
            ("nombre", true)    => consulta.OrderByDescending(e => e.Nombre),
            ("promedio", false) => consulta.OrderBy(e => e.Notas.Average(n => (decimal?)n.Valor)),
            ("promedio", true)  => consulta.OrderByDescending(e => e.Notas.Average(n => (decimal?)n.Valor)),
            (_, true)           => consulta.OrderByDescending(e => e.Id),
            _                   => consulta.OrderBy(e => e.Id)
        };

        var proyeccion = Proyectar(consulta);
        return await proyeccion.PaginarAsync(query, ct);
    }

    public async Task<EstudianteDto> ObtenerAsync(int id, CancellationToken ct = default) =>
        await db.Estudiantes.AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EstudianteDto(e.Id, e.Nombre, e.Notas.Count,
                e.Notas.Any() ? (decimal?)Math.Round(e.Notas.Average(n => n.Valor), 1) : null))
            .FirstOrDefaultAsync(ct)
        ?? throw new NoEncontradoException($"No existe un estudiante con id {id}.");

    public async Task<EstudianteDto> CrearAsync(PersonaGuardarDto dto, CancellationToken ct = default)
    {
        var entidad = new Estudiante { Nombre = dto.Nombre.Trim() };
        db.Estudiantes.Add(entidad);
        await db.SaveChangesAsync(ct);
        return new EstudianteDto(entidad.Id, entidad.Nombre, 0, null);
    }

    public async Task<EstudianteDto> ActualizarAsync(int id, PersonaGuardarDto dto, CancellationToken ct = default)
    {
        var entidad = await db.Estudiantes.FindAsync([id], ct)
            ?? throw new NoEncontradoException($"No existe un estudiante con id {id}.");
        entidad.Nombre = dto.Nombre.Trim();
        await db.SaveChangesAsync(ct);
        return await ObtenerAsync(id, ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var entidad = await db.Estudiantes.FindAsync([id], ct)
            ?? throw new NoEncontradoException($"No existe un estudiante con id {id}.");

        if (await db.Notas.AnyAsync(n => n.IdEstudiante == id, ct))
            throw new ConflictoException("No se puede eliminar el estudiante porque tiene notas registradas.");

        db.Estudiantes.Remove(entidad);
        await db.SaveChangesAsync(ct);
    }

    private static IQueryable<EstudianteDto> Proyectar(IQueryable<Estudiante> consulta) =>
        consulta.Select(e => new EstudianteDto(
            e.Id,
            e.Nombre,
            e.Notas.Count,
            e.Notas.Any() ? (decimal?)Math.Round(e.Notas.Average(n => n.Valor), 1) : null));
}
