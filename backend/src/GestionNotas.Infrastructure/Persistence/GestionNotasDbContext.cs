using GestionNotas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionNotas.Infrastructure.Persistence;

public class GestionNotasDbContext(DbContextOptions<GestionNotasDbContext> options) : DbContext(options)
{
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Profesor> Profesores => Set<Profesor>();
    public DbSet<Nota> Notas => Set<Nota>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // El mapeo coincide con database/script.sql (tablas en singular, esquema dbo).
        modelBuilder.Entity<Estudiante>(e =>
        {
            e.ToTable("Estudiante");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Profesor>(e =>
        {
            e.ToTable("Profesor");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Nota>(e =>
        {
            e.ToTable("Nota", t => t.HasCheckConstraint("CK_Nota_Valor", "[Valor] BETWEEN 0 AND 5"));
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            e.Property(x => x.Valor).HasPrecision(3, 1);

            // Llaves foráneas. Restrict: no se borra en cascada; la regla la valida el servicio (409).
            e.HasOne(x => x.Profesor).WithMany(p => p.Notas)
             .HasForeignKey(x => x.IdProfesor)
             .HasConstraintName("FK_Nota_Profesor")
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Estudiante).WithMany(s => s.Notas)
             .HasForeignKey(x => x.IdEstudiante)
             .HasConstraintName("FK_Nota_Estudiante")
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
