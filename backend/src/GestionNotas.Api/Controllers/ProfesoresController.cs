using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionNotas.Api.Controllers;

[ApiController]
[Route("api/profesores")]
[Produces("application/json")]
public class ProfesoresController(IProfesorService servicio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<ProfesorDto>>> Listar([FromQuery] PaginacionQuery query, CancellationToken ct)
        => Ok(await servicio.ListarAsync(query, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProfesorDto>> Obtener(int id, CancellationToken ct)
        => Ok(await servicio.ObtenerAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProfesorDto>> Crear(PersonaGuardarDto dto, CancellationToken ct)
    {
        var creado = await servicio.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProfesorDto>> Actualizar(int id, PersonaGuardarDto dto, CancellationToken ct)
        => Ok(await servicio.ActualizarAsync(id, dto, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await servicio.EliminarAsync(id, ct);
        return NoContent();
    }
}
