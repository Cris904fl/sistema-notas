using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;

namespace GestionNotas.Application.Interfaces;

public interface IEstudianteService
{
    Task<ResultadoPaginado<EstudianteDto>> ListarAsync(PaginacionQuery query, CancellationToken ct = default);
    Task<EstudianteDto> ObtenerAsync(int id, CancellationToken ct = default);
    Task<EstudianteDto> CrearAsync(PersonaGuardarDto dto, CancellationToken ct = default);
    Task<EstudianteDto> ActualizarAsync(int id, PersonaGuardarDto dto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
}

public interface IProfesorService
{
    Task<ResultadoPaginado<ProfesorDto>> ListarAsync(PaginacionQuery query, CancellationToken ct = default);
    Task<ProfesorDto> ObtenerAsync(int id, CancellationToken ct = default);
    Task<ProfesorDto> CrearAsync(PersonaGuardarDto dto, CancellationToken ct = default);
    Task<ProfesorDto> ActualizarAsync(int id, PersonaGuardarDto dto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
}

public interface INotaService
{
    Task<ResultadoPaginado<NotaDto>> ListarAsync(PaginacionQuery query, int? idEstudiante, int? idProfesor, CancellationToken ct = default);
    Task<IReadOnlyList<NotaDto>> ExportarAsync(PaginacionQuery query, int? idEstudiante, int? idProfesor, CancellationToken ct = default);
    Task<NotaDto> ObtenerAsync(int id, CancellationToken ct = default);
    Task<NotaDto> CrearAsync(NotaGuardarDto dto, CancellationToken ct = default);
    Task<NotaDto> ActualizarAsync(int id, NotaGuardarDto dto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
}

public interface IResumenService
{
    Task<ResumenDto> ObtenerAsync(int cantidadMejores = 5, CancellationToken ct = default);
}
