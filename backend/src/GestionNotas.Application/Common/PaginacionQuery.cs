namespace GestionNotas.Application.Common;

/// <summary>Parámetros de paginación, búsqueda y orden que llegan por query string.</summary>
public class PaginacionQuery
{
    private const int TamanoMaximo = 50;
    private int _pagina = 1;
    private int _tamano = 10;

    public int Pagina { get => _pagina; set => _pagina = value < 1 ? 1 : value; }
    public int Tamano { get => _tamano; set => _tamano = value < 1 ? 10 : Math.Min(value, TamanoMaximo); }

    /// <summary>Texto para filtrar por nombre.</summary>
    public string? Buscar { get; set; }

    /// <summary>Campo de orden, por ejemplo "nombre" o "valor".</summary>
    public string? OrdenarPor { get; set; }
    public bool Descendente { get; set; }
}
