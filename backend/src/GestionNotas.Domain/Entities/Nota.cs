namespace GestionNotas.Domain.Entities;

public class Nota
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdProfesor { get; set; }
    public int IdEstudiante { get; set; }
    public decimal Valor { get; set; }

    public Profesor? Profesor { get; set; }
    public Estudiante? Estudiante { get; set; }
}
