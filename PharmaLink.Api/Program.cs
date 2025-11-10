// Imports básicos (ASP.NET, EF y nuestras clases)
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmaLink.Api;
using PharmaLink.Data;

var builder = WebApplication.CreateBuilder(args);

// Puerto HTTP
builder.WebHost.UseUrls("http://localhost:5000");

// Registrar servicios principales (Startup)
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// HttpClient Hospital (mover ANTES de Build)
builder.Services.AddHttpClient("HospitalApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5100"); // ajustar si cambia
    client.DefaultRequestHeaders.Add("X-API-KEY", "HOSPITAL_KEY");
});

var app = builder.Build();

app.UseCors("AllowAll");
startup.Configure(app, app.Environment);

// Seeding + migraciones
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<PharmaLinkContext>();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    try { ctx.Database.Migrate(); } catch { /* log si quieres */ }

    var isInMemory = ctx.Database.ProviderName?.Contains("InMemory", StringComparison.OrdinalIgnoreCase) == true;
    var seedOnStart = cfg.GetValue<bool>("Demo:SeedOnStart");
    if (isInMemory || seedOnStart)
        DataInitializer.Initialize(ctx);
}

Console.WriteLine("\nServidor: http://localhost:5000");
Console.WriteLine("Swagger:  http://localhost:5000/swagger\n");

app.Run();





