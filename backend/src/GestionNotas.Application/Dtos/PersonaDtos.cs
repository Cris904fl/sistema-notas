using System.ComponentModel.DataAnnotations;

namespace GestionNotas.Application.Dtos;

/// <summary>Datos para crear o actualizar un estudiante o un profesor.</summary>
public record PersonaGuardarDto(
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    string Nombre);

public record EstudianteDto(int Id, string Nombre, int CantidadNotas, decimal? Promedio);

public record ProfesorDto(int Id, string Nombre, int CantidadNotas);
