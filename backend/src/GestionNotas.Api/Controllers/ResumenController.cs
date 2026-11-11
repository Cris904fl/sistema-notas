using GestionNotas.Application.Dtos;
using GestionNotas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionNotas.Api.Controllers;

[ApiController]
[Route("api/resumen")]
[Produces("application/json")]
public class ResumenController(IResumenService servicio) : ControllerBase
{
    /// <summary>Indicadores del tablero: totales, promedio general, aprobación y mejores promedios.</summary>
    [HttpGet]
    public async Task<ActionResult<ResumenDto>> Obtener([FromQuery] int mejores = 5, CancellationToken ct = default)
        => Ok(await servicio.ObtenerAsync(Math.Clamp(mejores, 1, 20), ct));
}
