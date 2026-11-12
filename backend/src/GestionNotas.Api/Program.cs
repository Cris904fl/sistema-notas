using GestionNotas.Api.Middleware;
using GestionNotas.Api.Salud;
using GestionNotas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

const string CorsAngular = "Angular";

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck<BaseDatosHealthCheck>("base-de-datos", tags: ["listo"]);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy(CorsAngular, p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? ["http://localhost:4200"])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ManejoErroresMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.DocumentTitle = "Gestión de Notas – API");
}

app.UseCors(CorsAngular);
app.MapControllers();

// /salud/vivo: el proceso responde (no consulta dependencias).
// /salud:      además verifica la conexión con SQL Server.
app.MapHealthChecks("/salud/vivo", new() { Predicate = _ => false });
app.MapHealthChecks("/salud", new() { ResponseWriter = RespuestaSalud.EscribirAsync });

app.Run();
