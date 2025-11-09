using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PharmaLink.Data;

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

            // Registrar DbContext — si no hay connection string, usar InMemory (para desarrollo/pruebas)
            var conn = Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(conn))
            {
                services.AddDbContext<PharmaLinkContext>(options =>
                    options.UseSqlServer(conn));
            }
            else
            {
                // En desarrollo sin conexión, usar In-Memory database (perfecto para pruebas)
                services.AddDbContext<PharmaLinkContext>(options =>
                    options.UseInMemoryDatabase("PharmaLinkDev"));
            }

            // Registrar HttpClient para la API del hospital
            services.AddHttpClient("HospitalApi", client =>
            {
                var baseUrl = Configuration["HospitalApi:BaseUrl"];
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    client.BaseAddress = new System.Uri(baseUrl);
                }
            });
        }

    // Método para configurar la tubería HTTP (middlewares, routing, etc.). Aquí pongo swagger, https, auth, etc.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Solo redirigir a HTTPS en producción (en desarrollo, evita problemas con certificado auto-firmado)
                app.UseHttpsRedirection();
            }

            // Middleware de API Key para proteger endpoints sensibles (recetas, dispensaciones, reposiciones)
            // TEMPORALMENTE DESHABILITADO PARA DEBUGGING
            // app.UseMiddleware<Middleware.ApiKeyMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
