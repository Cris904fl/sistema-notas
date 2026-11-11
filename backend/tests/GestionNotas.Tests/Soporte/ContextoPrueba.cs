using GestionNotas.Domain.Entities;
using GestionNotas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Tests.Soporte;

/// <summary>Crea un contexto en memoria, aislado por prueba, con datos conocidos.</summary>
public static class ContextoPrueba
{
    public static GestionNotasDbContext Crear(bool conDatos = true)
    {
        var opciones = new DbContextOptionsBuilder<GestionNotasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())   // nombre único: ninguna prueba comparte datos
            .Options;

        var db = new GestionNotasDbContext(opciones);
        if (conDatos) Sembrar(db);
        return db;
    }

    private static void Sembrar(GestionNotasDbContext db)
    {
        db.Profesores.AddRange(
            new Profesor { Id = 1, Nombre = "Ana María Rodríguez" },
            new Profesor { Id = 2, Nombre = "Carlos Andrés Gómez" },
            new Profesor { Id = 3, Nombre = "Luisa Fernanda Torres" });   // sin notas

        db.Estudiantes.AddRange(
            new Estudiante { Id = 1, Nombre = "Juan Pablo Martínez" },
            new Estudiante { Id = 2, Nombre = "Valentina Herrera" },
            new Estudiante { Id = 3, Nombre = "Santiago López" });        // sin notas

        db.Notas.AddRange(
            new Nota { Id = 1, Nombre = "Parcial 1",     IdProfesor = 1, IdEstudiante = 1, Valor = 4.5m },
            new Nota { Id = 2, Nombre = "Taller de SQL", IdProfesor = 2, IdEstudiante = 1, Valor = 3.0m },
            new Nota { Id = 3, Nombre = "Parcial 1",     IdProfesor = 1, IdEstudiante = 2, Valor = 2.5m });

        db.SaveChanges();
        db.ChangeTracker.Clear();   // que las pruebas lean desde la "base", no desde la memoria del contexto
    }
}
