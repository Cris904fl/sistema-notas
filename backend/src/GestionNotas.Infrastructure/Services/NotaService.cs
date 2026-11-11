using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Exceptions;
using GestionNotas.Application.Interfaces;
using GestionNotas.Domain.Entities;
using GestionNotas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Infrastructure.Services;

public class NotaService(GestionNotasDbContext db) : INotaService
{
    public async Task<ResultadoPaginado<NotaDto>> ListarAsync(PaginacionQuery query, int? idEstudiante, int? idProfesor,
                                                              CancellationToken ct = default)
    {
        var consulta = db.Notas.AsNoTracking();

        if (idEstudiante is not null) consulta = consulta.Where(n => n.IdEstudiante == idEstudiante);
        if (idProfesor is not null)   consulta = consulta.Where(n => n.IdProfesor == idProfesor);

        if (!string.IsNullOrWhiteSpace(query.Buscar))
        {
            var texto = query.Buscar.Trim();
            consulta = consulta.Where(n => n.Nombre.Contains(texto)
                                        || n.Estudiante!.Nombre.Contains(texto)
                                        || n.Profesor!.Nombre.Contains(texto));
        }

        consulta = (query.OrdenarPor?.ToLowerInvariant(), query.Descendente) switch
        {
            ("nombre", false)     => consulta.OrderBy(n => n.Nombre),
            ("nombre", true)      => consulta.OrderByDescending(n => n.Nombre),
            ("valor", false)      => consulta.OrderBy(n => n.Valor),
            ("valor", true)       => consulta.OrderByDescending(n => n.Valor),
            ("estudiante", false) => consulta.OrderBy(n => n.Estudiante!.Nombre),
            ("estudiante", true)  => consulta.OrderByDescending(n => n.Estudiante!.Nombre),
            ("profesor", false)   => consulta.OrderBy(n => n.Profesor!.Nombre),
            ("profesor", true)    => consulta.OrderByDescending(n => n.Profesor!.Nombre),
            (_, true)             => consulta.OrderByDescending(n => n.Id),
            _                     => consulta.OrderBy(n => n.Id)
        };

        var proyeccion = Proyectar(consulta);
        return await proyeccion.PaginarAsync(query, ct);
    }

    public async Task<NotaDto> ObtenerAsync(int id, CancellationToken ct = default) =>
        await Proyectar(db.Notas.AsNoTracking().Where(n => n.Id == id)).FirstOrDefaultAsync(ct)
        ?? throw new NoEncontradoException($"No existe una nota con id {id}.");

    public async Task<NotaDto> CrearAsync(NotaGuardarDto dto, CancellationToken ct = default)
    {
        await ValidarRelacionesAsync(dto, ct);
        var entidad = new Nota
        {
            Nombre = dto.Nombre.Trim(),
            IdEstudiante = dto.IdEstudiante,
            IdProfesor = dto.IdProfesor,
            Valor = Math.Round(dto.Valor, 1)
        };
        db.Notas.Add(entidad);
        await db.SaveChangesAsync(ct);
        return await ObtenerAsync(entidad.Id, ct);
    }

    public async Task<NotaDto> ActualizarAsync(int id, NotaGuardarDto dto, CancellationToken ct = default)
    {
        var entidad = await db.Notas.FindAsync([id], ct)
            ?? throw new NoEncontradoException($"No existe una nota con id {id}.");
        await ValidarRelacionesAsync(dto, ct);

        entidad.Nombre = dto.Nombre.Trim();
        entidad.IdEstudiante = dto.IdEstudiante;
        entidad.IdProfesor = dto.IdProfesor;
        entidad.Valor = Math.Round(dto.Valor, 1);
        await db.SaveChangesAsync(ct);
        return await ObtenerAsync(id, ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var entidad = await db.Notas.FindAsync([id], ct)
            ?? throw new NoEncontradoException($"No existe una nota con id {id}.");
        db.Notas.Remove(entidad);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Verifica que el estudiante y el profesor existan antes de guardar (mensaje claro en vez de error de FK).</summary>
    private async Task ValidarRelacionesAsync(NotaGuardarDto dto, CancellationToken ct)
    {
        if (!await db.Estudiantes.AnyAsync(e => e.Id == dto.IdEstudiante, ct))
            throw new ValidacionException($"El estudiante con id {dto.IdEstudiante} no existe.");
        if (!await db.Profesores.AnyAsync(p => p.Id == dto.IdProfesor, ct))
            throw new ValidacionException($"El profesor con id {dto.IdProfesor} no existe.");
    }

    private static IQueryable<NotaDto> Proyectar(IQueryable<Nota> consulta) =>
        consulta.Select(n => new NotaDto(
            n.Id, n.Nombre, n.Valor,
            n.IdEstudiante, n.Estudiante!.Nombre,
            n.IdProfesor, n.Profesor!.Nombre));
}
