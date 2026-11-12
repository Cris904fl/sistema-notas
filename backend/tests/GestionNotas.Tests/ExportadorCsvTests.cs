using System.Text;
using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;

namespace GestionNotas.Tests;

public class ExportadorCsvTests
{
    private static string Texto(byte[] bytes) => Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');

    [Fact]
    public void GenerarNotas_IncluyeEncabezadoYUsaComaDecimal()
    {
        var notas = new[] { new NotaDto(1, "Parcial 1", 4.5m, 1, "Juan Pablo Martínez", 2, "Ana María Rodríguez", []) };

        var lineas = Texto(ExportadorCsv.GenerarNotas(notas)).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("Id;Nota;Estudiante;Profesor;Valor;Estado", lineas[0]);
        Assert.Equal("1;Parcial 1;Juan Pablo Martínez;Ana María Rodríguez;4,5;Aprobada", lineas[1]);
    }

    [Fact]
    public void GenerarNotas_EmpiezaConBomParaQueExcelMuestreLasTildes()
    {
        var bytes = ExportadorCsv.GenerarNotas([]);
        Assert.Equal(Encoding.UTF8.GetPreamble(), bytes.Take(3));
    }

    [Fact]
    public void GenerarNotas_MarcaComoReprobadaUnaNotaMenorATres()
    {
        var notas = new[] { new NotaDto(2, "Quiz", 2.9m, 1, "Santiago López", 1, "Ana María Rodríguez", []) };
        Assert.EndsWith("2,9;Reprobada", Texto(ExportadorCsv.GenerarNotas(notas)).TrimEnd());
    }

    [Theory]
    [InlineData("Parcial 1", "Parcial 1")]
    [InlineData("Taller; parte 2", "\"Taller; parte 2\"")]
    [InlineData("Nota \"final\"", "\"Nota \"\"final\"\"\"")]
    public void Escapar_EncierraEntreComillasCuandoEsNecesario(string entrada, string esperado)
        => Assert.Equal(esperado, ExportadorCsv.Escapar(entrada));
}
