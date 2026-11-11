using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionNotas.Api.Controllers;

[ApiController]
[Route("api/notas")]
[Produces("application/json")]
public class NotasController(INotaService servicio) : ControllerBase
{
    /// <summary>Lista paginada de notas; se puede filtrar por estudiante o profesor.</summary>
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<NotaDto>>> Listar([FromQuery] PaginacionQuery query,
        [FromQuery] int? idEstudiante, [FromQuery] int? idProfesor, CancellationToken ct)
        => Ok(await servicio.ListarAsync(query, idEstudiante, idProfesor, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotaDto>> Obtener(int id, CancellationToken ct)
        => Ok(await servicio.ObtenerAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<NotaDto>> Crear(NotaGuardarDto dto, CancellationToken ct)
    {
        var creada = await servicio.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<NotaDto>> Actualizar(int id, NotaGuardarDto dto, CancellationToken ct)
        => Ok(await servicio.ActualizarAsync(id, dto, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await servicio.EliminarAsync(id, ct);
        return NoContent();
    }
}
