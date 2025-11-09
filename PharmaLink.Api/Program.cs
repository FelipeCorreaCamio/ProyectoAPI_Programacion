using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PharmaLink.Api;
using PharmaLink.Data;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en un puerto libre (3000 está ocupado). Usamos 5000.
builder.WebHost.UseUrls("http://localhost:5000");

// Usar la clase Startup existente
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configurar middlewares/pipeline mediante Startup
startup.Configure(app, app.Environment);

// Inicializar datos de ejemplo si la BD está vacía (InMemory por defecto)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var ctx = scope.ServiceProvider.GetRequiredService<PharmaLinkContext>();
        DataInitializer.Initialize(ctx);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Error inicializando la base de datos");
    }
}

Console.WriteLine("\n✅ Servidor iniciado en http://localhost:5000");
Console.WriteLine("📚 Documentación: http://localhost:5000/swagger");
Console.WriteLine("💊 Medicamentos: http://localhost:5000/api/medicamentos");
Console.WriteLine("Presiona Ctrl+C para detener\n");

app.Run();





