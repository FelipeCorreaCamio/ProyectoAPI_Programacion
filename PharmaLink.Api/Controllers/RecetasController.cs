using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace PharmaLink.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetasController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RecetasController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

    // Validar receta (POST api/recetas/validate) — le pedimos al hospital que confirme si la receta está ok
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("codigoReceta", out var codigoElem))
                return BadRequest(new { code = "missing_field", message = "codigoReceta is required" });

            var codigo = codigoElem.GetString();
            var baseUrl = _configuration["HospitalApi:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                return StatusCode(503, new { code = "hospital_api_unconfigured", message = "Hospital API not configured" });
            }

            var client = _httpClientFactory.CreateClient("HospitalApi");

            // Ejemplo: GET /recetas/{codigo}/validate — la ruta real depende de cómo lo haga el grupo del hospital
            var resp = await client.GetAsync($"/recetas/{codigo}/validate");
            if (!resp.IsSuccessStatusCode)
            {
                return StatusCode((int)resp.StatusCode, new { code = "hospital_api_error", message = "Error contacting hospital API" });
            }

            var content = await resp.Content.ReadAsStringAsync();
            // Intento parsear la respuesta flexible que mande el hospital (puede variar)
            try
            {
                var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("valid", out var validEl) && validEl.GetBoolean())
                {
                    return Ok(new { valid = true });
                }
            }
            catch
            {
                // si no trae lo que esperamos, seguimos y devolvemos invalid
            }

            return BadRequest(new { valid = false });
        }
    }
}
