using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Exceptions;
using GestionNotas.Application.Interfaces;
using GestionNotas.Domain.Entities;
using GestionNotas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Infrastructure.Services;

public class ProfesorService(GestionNotasDbContext db) : IProfesorService
{
    public async Task<ResultadoPaginado<ProfesorDto>> ListarAsync(PaginacionQuery query, CancellationToken ct = default)
    {
        var consulta = db.Profesores.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Buscar))
            consulta = consulta.Where(p => p.Nombre.Contains(query.Buscar.Trim()));

        consulta = (query.OrdenarPor?.ToLowerInvariant(), query.Descendente) switch
        {
            ("nombre", false) => consulta.OrderBy(p => p.Nombre),
            ("nombre", true)  => consulta.OrderByDescending(p => p.Nombre),
            (_, true)         => consulta.OrderByDescending(p => p.Id),
            _                 => consulta.OrderBy(p => p.Id)
        };

        var proyeccion = consulta.Select(p => new ProfesorDto(p.Id, p.Nombre, p.Notas.Count));
        return await proyeccion.PaginarAsync(query, ct);
    }

    public async Task<ProfesorDto> ObtenerAsync(int id, CancellationToken ct = default) =>
        await db.Profesores.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProfesorDto(p.Id, p.Nombre, p.Notas.Count))
            .FirstOrDefaultAsync(ct)
        ?? throw new NoEncontradoException($"No existe un profesor con id {id}.");

    public async Task<ProfesorDto> CrearAsync(PersonaGuardarDto dto, CancellationToken ct = default)
    {
        var entidad = new Profesor { Nombre = dto.Nombre.Trim() };
        db.Profesores.Add(entidad);
        await db.SaveChangesAsync(ct);
        return new ProfesorDto(entidad.Id, entidad.Nombre, 0);
    }

    public async Task<ProfesorDto> ActualizarAsync(int id, PersonaGuardarDto dto, CancellationToken ct = default)
    {
        var entidad = await db.Profesores.FindAsync([id], ct)
            ?? throw new NoEncontradoException($"No existe un profesor con id {id}.");
        entidad.Nombre = dto.Nombre.Trim();
        await db.SaveChangesAsync(ct);
        return await ObtenerAsync(id, ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var entidad = await db.Profesores.FindAsync([id], ct)
            ?? throw new NoEncontradoException($"No existe un profesor con id {id}.");

        if (await db.Notas.AnyAsync(n => n.IdProfesor == id, ct))
            throw new ConflictoException("No se puede eliminar el profesor porque tiene notas registradas.");

        db.Profesores.Remove(entidad);
        await db.SaveChangesAsync(ct);
    }
}
