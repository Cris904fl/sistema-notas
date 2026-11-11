using System.ComponentModel.DataAnnotations;

namespace GestionNotas.Application.Dtos;

public record NotaGuardarDto(
    [Required(ErrorMessage = "El nombre de la nota es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    string Nombre,
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un profesor.")]
    int IdProfesor,
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante.")]
    int IdEstudiante,
    [Range(typeof(decimal), "0", "5", ErrorMessage = "El valor debe estar entre 0,0 y 5,0.")]
    decimal Valor);

/// <summary>Nota con los nombres del estudiante y del profesor, para mostrar en la tabla.</summary>
public record NotaDto(int Id, string Nombre, decimal Valor,
                      int IdEstudiante, string Estudiante,
                      int IdProfesor, string Profesor);
