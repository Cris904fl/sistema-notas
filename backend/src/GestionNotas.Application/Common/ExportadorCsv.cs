using System.Globalization;
using System.Text;
using GestionNotas.Application.Dtos;

namespace GestionNotas.Application.Common;

/// <summary>
/// Genera el CSV de notas pensado para Excel en español: separador ";", coma decimal
/// y BOM UTF-8 para que las tildes se vean bien al abrirlo.
/// </summary>
public static class ExportadorCsv
{
    private const char Separador = ';';
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-CO");

    public static byte[] GenerarNotas(IEnumerable<NotaDto> notas)
    {
        var csv = new StringBuilder();
        csv.AppendLine(string.Join(Separador, "Id", "Nota", "Estudiante", "Profesor", "Valor", "Estado"));

        foreach (var n in notas)
        {
            csv.AppendLine(string.Join(Separador,
                n.Id.ToString(Cultura),
                Escapar(n.Nombre),
                Escapar(n.Estudiante),
                Escapar(n.Profesor),
                n.Valor.ToString("0.0", Cultura),
                n.Valor >= 3 ? "Aprobada" : "Reprobada"));
        }

        var bom = Encoding.UTF8.GetPreamble();
        return [.. bom, .. Encoding.UTF8.GetBytes(csv.ToString())];
    }

    /// <summary>Encierra el texto entre comillas si contiene el separador, comillas o saltos de línea (RFC 4180).</summary>
    public static string Escapar(string texto)
    {
        if (texto.IndexOfAny([Separador, '"', '\n', '\r']) < 0) return texto;
        return $"\"{texto.Replace("\"", "\"\"")}\"";
    }
}
