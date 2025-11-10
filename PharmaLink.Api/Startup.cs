using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PharmaLink.Data;
using System.IO;

namespace PharmaLink.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Registrar DbContext — intenta usar ConnectionString; si no hay, fallback inteligente
            var conn = Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(conn))
            {
                // Detectar si la cadena apunta a SQLite (ej: termina en .db o contiene Data Source=)
                if (conn.Contains(".db", StringComparison.OrdinalIgnoreCase) || conn.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    services.AddDbContext<PharmaLinkContext>(options =>
                        options.UseSqlite(conn));
                }
                else
                {
                    services.AddDbContext<PharmaLinkContext>(options =>
                        options.UseSqlServer(conn));
                }
            }
            else
            {
                // Sin connection string: si existe pharmalink.db, usar SQLite; si no, InMemory
                var sqliteFileInCwd = Path.Combine(Directory.GetCurrentDirectory(), "pharmalink.db");
                var sqliteFileInBase = Path.Combine(AppContext.BaseDirectory, "pharmalink.db");
                if (File.Exists(sqliteFileInCwd) || File.Exists(sqliteFileInBase))
                {
                    services.AddDbContext<PharmaLinkContext>(options =>
                        options.UseSqlite("Data Source=pharmalink.db"));
                }
                else
                {
                    services.AddDbContext<PharmaLinkContext>(options =>
                        options.UseInMemoryDatabase("PharmaLinkDev"));
                }
            }

            // Registrar HttpClient para la API del hospital
            services.AddHttpClient("HospitalApi", (sp, client) =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                var baseUrl = cfg["HospitalApi:BaseUrl"];
                if (!string.IsNullOrWhiteSpace(baseUrl))
                    client.BaseAddress = new Uri(baseUrl);

                var apiKey = cfg["HospitalApi:ApiKey"];
                if (!string.IsNullOrEmpty(apiKey))
                    client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-KEY", apiKey);
            });
        }

        // Método para configurar la tubería HTTP (middlewares, routing, etc.). Aquí pongo swagger, https, auth, etc.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Quitar si no escuchamos en https
            // app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PharmaLink API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
