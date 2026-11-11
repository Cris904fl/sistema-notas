namespace GestionNotas.Application.Dtos;

/// <summary>Indicadores generales que devuelve el procedimiento dbo.sp_ResumenGeneral.</summary>
public class IndicadoresDto
{
    public int TotalEstudiantes { get; set; }
    public int TotalProfesores { get; set; }
    public int TotalNotas { get; set; }
    public decimal? PromedioGeneral { get; set; }
    public decimal? PorcentajeAprobacion { get; set; }
    public decimal? NotaMaxima { get; set; }
    public decimal? NotaMinima { get; set; }
}

/// <summary>Fila de las vistas dbo.vw_PromedioEstudiante y dbo.vw_PromedioProfesor.</summary>
public class PromedioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CantidadNotas { get; set; }
    public decimal? Promedio { get; set; }
}

public record ResumenDto(
    IndicadoresDto Indicadores,
    IReadOnlyList<PromedioDto> MejoresEstudiantes,
    IReadOnlyList<PromedioDto> PromedioPorProfesor);
