using PharmaLink.Api;

var builder = WebApplication.CreateBuilder(args);

// Uso la clase Startup para dejar la configuración acá (servicios y pipeline), así lo organizo yo
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
