using GestionNotas.Application.Interfaces;
using GestionNotas.Infrastructure.Persistence;
using GestionNotas.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionNotas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GestionNotasDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("GestionNotas")));

        services.AddScoped<IEstudianteService, EstudianteService>();
        services.AddScoped<IProfesorService, ProfesorService>();
        services.AddScoped<INotaService, NotaService>();
        return services;
    }
}
