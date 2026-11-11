using GestionNotas.Application.Common;
using GestionNotas.Application.Dtos;
using GestionNotas.Application.Exceptions;
using GestionNotas.Infrastructure.Services;
using GestionNotas.Tests.Soporte;

namespace GestionNotas.Tests.Servicios;

public class ProfesorServiceTests
{
    [Fact]
    public async Task Listar_OrdenaPorNombreDescendente()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new ProfesorService(db);

        var resultado = await servicio.ListarAsync(new PaginacionQuery { OrdenarPor = "nombre", Descendente = true });

        Assert.Equal(
            new[] { "Luisa Fernanda Torres", "Carlos Andrés Gómez", "Ana María Rodríguez" },
            resultado.Elementos.Select(p => p.Nombre));
    }

    [Fact]
    public async Task Crear_DevuelveElProfesorConCeroNotas()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new ProfesorService(db);

        var creado = await servicio.CrearAsync(new PersonaGuardarDto("Diana Paola Ruiz"));

        Assert.Equal(0, creado.CantidadNotas);
        Assert.Equal(4, db.Profesores.Count());
    }

    [Fact]
    public async Task Eliminar_LanzaConflicto_SiElProfesorTieneNotas()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new ProfesorService(db);

        await Assert.ThrowsAsync<ConflictoException>(() => servicio.EliminarAsync(1));
    }

    [Fact]
    public async Task Eliminar_LanzaNoEncontrado_SiNoExiste()
    {
        using var db = ContextoPrueba.Crear();
        var servicio = new ProfesorService(db);

        await Assert.ThrowsAsync<NoEncontradoException>(() => servicio.EliminarAsync(999));
    }
}
