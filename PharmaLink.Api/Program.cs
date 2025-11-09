// Imports básicos (ASP.NET, EF y nuestras clases)
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmaLink.Api; // Startup
using PharmaLink.Data; // DbContext + seeds

// Arranque de la app (config, logs, etc.)
var builder = WebApplication.CreateBuilder(args);

// Puerto 5000
builder.WebHost.UseUrls("http://localhost:5000");

// Registramos servicios con Startup (DB, Swagger, etc.)
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// Construye la app
var app = builder.Build();

// Configura middlewares y rutas
startup.Configure(app, app.Environment);

// Semillas de datos demo:
// InMemory = siempre; SQLite/SQL = si Demo:SeedOnStart=true
using (var scope = app.Services.CreateScope())
{
    try
    {
        var ctx = scope.ServiceProvider.GetRequiredService<PharmaLinkContext>();
        var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Asegurar que la base exista y tenga migraciones aplicadas (auto-migrate)
        try
        {
            ctx.Database.Migrate();
        }
        catch (Exception mex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogError(mex, "Error aplicando migraciones automáticamente");
        }

        // InMemory o Demo:SeedOnStart => sembrar
        var isInMemory = ctx.Database.ProviderName?.Contains("InMemory", StringComparison.OrdinalIgnoreCase) == true;
        var seedOnStart = cfg.GetValue<bool>("Demo:SeedOnStart");
        if (isInMemory || seedOnStart)
        {
            DataInitializer.Initialize(ctx);
        }
    }
    catch (Exception ex)
    {
        // Si falla, log y seguimos
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Error inicializando la base de datos");
    }
}

// Links útiles
Console.WriteLine("\n✅ Servidor iniciado en http://localhost:5000");
Console.WriteLine("📚 Swagger: http://localhost:5000/swagger (abrí y probá los endpoints)");
Console.WriteLine("💊 GET medicamentos: http://localhost:5000/api/medicamentos");
Console.WriteLine("(Tip) Si no ves datos y querés demo: en appsettings.Development.json poné Demo.SeedOnStart=true\n");

app.Run();





