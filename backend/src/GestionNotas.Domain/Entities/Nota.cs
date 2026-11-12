namespace GestionNotas.Domain.Entities;

public class Nota
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdProfesor { get; set; }
    public int IdEstudiante { get; set; }
    public decimal Valor { get; set; }

    /// <summary>Versión de la fila (ROWVERSION). SQL Server la cambia en cada actualización.</summary>
    public byte[] RowVersion { get; set; } = [];

    public Profesor? Profesor { get; set; }
    public Estudiante? Estudiante { get; set; }
}
