using GestionNotas.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Infrastructure.Persistence;

public static class QueryableExtensions
{
    /// <summary>Cuenta el total y trae solo la página pedida; ambas consultas se ejecutan en SQL Server.</summary>
    public static async Task<ResultadoPaginado<T>> PaginarAsync<T>(
        this IQueryable<T> consulta, PaginacionQuery query, CancellationToken ct)
    {
        var total = await consulta.CountAsync(ct);
        var elementos = await consulta
            .Skip((query.Pagina - 1) * query.Tamano)
            .Take(query.Tamano)
            .ToListAsync(ct);
        return new ResultadoPaginado<T>(elementos, query.Pagina, query.Tamano, total);
    }
}
