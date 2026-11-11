using GestionNotas.Application.Common;

namespace GestionNotas.Tests;

public class PaginacionQueryTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public void Pagina_NuncaEsMenorQueUno(int valor, int esperado)
    {
        var query = new PaginacionQuery { Pagina = valor };
        Assert.Equal(esperado, query.Pagina);
    }

    [Theory]
    [InlineData(0, 10)]     // valor inválido: usa el tamaño por defecto
    [InlineData(25, 25)]
    [InlineData(500, 50)]   // nunca más de 50 por página, para proteger la base de datos
    public void Tamano_SeAjustaAlRangoPermitido(int valor, int esperado)
    {
        var query = new PaginacionQuery { Tamano = valor };
        Assert.Equal(esperado, query.Tamano);
    }

    [Fact]
    public void ResultadoPaginado_CalculaElTotalDePaginas()
    {
        var resultado = new ResultadoPaginado<int>([], Pagina: 1, Tamano: 5, TotalElementos: 11);
        Assert.Equal(3, resultado.TotalPaginas);
    }
}
