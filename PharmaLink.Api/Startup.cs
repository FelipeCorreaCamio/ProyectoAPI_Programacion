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

    // Método que se ejecuta para registrar servicios. Aquí meto controladores, swagger y otras cosas básicas.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Registro el DbContext solo si hay connection string (no quiero que falle si todavía no la ponen)
            var conn = Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(conn))
            {
                services.AddDbContext<PharmaLinkContext>(options =>
                    options.UseSqlServer(conn));
            }
            // Registro un HttpClient para la API del hospital — la base URL la saco de appsettings
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

            app.UseHttpsRedirection();

            // Middleware de API Key para proteger endpoints sensibles (recetas, dispensaciones, reposiciones)
            app.UseMiddleware<Middleware.ApiKeyMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
