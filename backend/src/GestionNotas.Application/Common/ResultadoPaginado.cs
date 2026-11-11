namespace GestionNotas.Application.Common;

public record ResultadoPaginado<T>(IReadOnlyList<T> Elementos, int Pagina, int Tamano, int TotalElementos)
{
    public int TotalPaginas => Tamano == 0 ? 0 : (int)Math.Ceiling(TotalElementos / (double)Tamano);
}
