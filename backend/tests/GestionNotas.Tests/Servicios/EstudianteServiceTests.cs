using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Exceptions;
using GestionNotas.Infrastructure.Services;
using GestionNotas.Tests.Soporte;

namespace GestionNotas.Tests.Servicios;

public class EstudianteServiceTests
{
    [Fact]
    public async Task Listar_CalculaCantidadDeNotasYPromedio()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery { OrdenarPor = "nombre" });

        var juan = resultado.Elementos.Single(e => e.Id == 1);
        Assert.Equal(2, juan.CantidadNotas);
        Assert.Equal(3.8m, juan.Promedio);          // (4,5 + 3,0) / 2 = 3,75 → 3,8

        var santiago = resultado.Elementos.Single(e => e.Id == 3);
        Assert.Equal(0, santiago.CantidadNotas);
        Assert.Null(santiago.Promedio);             // sin notas no hay promedio
    }

    [Fact]
    public async Task Listar_FiltraPorTextoDeBusqueda()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery { Buscar = "Valen" });

        Assert.Single(resultado.Elementos);
        Assert.Equal("Valentina Herrera", resultado.Elementos[0].Nombre);
    }

    [Fact]
    public async Task Listar_PaginaCorrectamente()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery { Pagina = 2, Tamano = 2 });

        Assert.Equal(3, resultado.TotalElementos);
        Assert.Equal(2, resultado.TotalPaginas);
        Assert.Single(resultado.Elementos);         // en la página 2 queda un solo estudiante
        Assert.Equal(3, resultado.Elementos[0].Id);
    }

    [Fact]
    public async Task Crear_GuardaElNombreSinEspaciosSobrantes()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        var creado = await servicio.CrearAsync(new PersonaGuardarDto("   María José Pérez  "));

        Assert.True(creado.Id > 0);
        Assert.Equal("María José Pérez", creado.Nombre);
        Assert.Equal(4, db.Estudiantes.Count());
    }

    [Fact]
    public async Task Actualizar_CambiaElNombre()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        var actualizado = await servicio.ActualizarAsync(3, new PersonaGuardarDto("Santiago López Ruiz"));

        Assert.Equal("Santiago López Ruiz", actualizado.Nombre);
    }

    [Fact]
    public async Task Obtener_LanzaNoEncontrado_SiNoExiste()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        await Assert.ThrowsAsync<NoEncontradoException>(() => servicio.ObtenerAsync(999));
    }

    [Fact]
    public async Task Eliminar_LanzaConflicto_SiElEstudianteTieneNotas()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        var error = await Assert.ThrowsAsync<ConflictoException>(() => servicio.EliminarAsync(1));

        Assert.Contains("tiene notas registradas", error.Message);
        Assert.Equal(3, db.Estudiantes.Count());    // no se borró nada
    }

    [Fact]
    public async Task Eliminar_BorraAlEstudiante_SiNoTieneNotas()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new EstudianteService(db);

        await servicio.EliminarAsync(3);

        Assert.Equal(2, db.Estudiantes.Count());
        Assert.DoesNotContain(db.Estudiantes, e => e.Id == 3);
    }
}
