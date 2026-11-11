using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Exceptions;
using GestionNotas.Infrastructure.Services;
using GestionNotas.Tests.Soporte;

namespace GestionNotas.Tests.Servicios;

public class NotaServiceTests
{
    [Fact]
    public async Task Listar_IncluyeLosNombresDelEstudianteYDelProfesor()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery(), null, null);

        var nota = resultado.Elementos.Single(n => n.Id == 2);
        Assert.Equal("Juan Pablo Martínez", nota.Estudiante);
        Assert.Equal("Carlos Andrés Gómez", nota.Profesor);
    }

    [Fact]
    public async Task Listar_FiltraPorEstudiante()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery(), idEstudiante: 1, idProfesor: null);

        Assert.Equal(2, resultado.TotalElementos);
        Assert.All(resultado.Elementos, n => Assert.Equal(1, n.IdEstudiante));
    }

    [Fact]
    public async Task Listar_BuscaTambienPorNombreDelProfesor()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery { Buscar = "Carlos" }, null, null);

        Assert.Single(resultado.Elementos);
        Assert.Equal("Taller de SQL", resultado.Elementos[0].Nombre);
    }

    [Fact]
    public async Task Listar_OrdenaPorValorDeMayorAMenor()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery { OrdenarPor = "valor", Descendente = true }, null, null);

        Assert.Equal(new[] { 4.5m, 3.0m, 2.5m }, resultado.Elementos.Select(n => n.Valor));
    }

    [Fact]
    public async Task Crear_RedondeaElValorAUnDecimal()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var creada = await servicio.CrearAsync(new NotaGuardarDto("Quiz", IdProfesor: 2, IdEstudiante: 3, Valor: 4.26m));

        Assert.Equal(4.3m, creada.Valor);
        Assert.Equal("Santiago López", creada.Estudiante);
    }

    [Fact]
    public async Task Crear_LanzaValidacion_SiElEstudianteNoExiste()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var error = await Assert.ThrowsAsync<ValidacionException>(() =>
            servicio.CrearAsync(new NotaGuardarDto("Quiz", IdProfesor: 1, IdEstudiante: 999, Valor: 4m)));

        Assert.Contains("estudiante", error.Message);
    }

    [Fact]
    public async Task Crear_LanzaValidacion_SiElProfesorNoExiste()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        await Assert.ThrowsAsync<ValidacionException>(() =>
            servicio.CrearAsync(new NotaGuardarDto("Quiz", IdProfesor: 999, IdEstudiante: 1, Valor: 4m)));
    }

    [Fact]
    public async Task Actualizar_CambiaElValorYElProfesor()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        var actualizada = await servicio.ActualizarAsync(3, new NotaGuardarDto("Parcial 1", IdProfesor: 2, IdEstudiante: 2, Valor: 3.5m));

        Assert.Equal(3.5m, actualizada.Valor);
        Assert.Equal("Carlos Andrés Gómez", actualizada.Profesor);
    }

    [Fact]
    public async Task Eliminar_BorraLaNota()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new NotaService(db);

        await servicio.EliminarAsync(3);

        Assert.Equal(2, db.Notas.Count());
    }
}
