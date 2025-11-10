using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PharmaLink.Api.Models.Dto;
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
    public async Task<IActionResult> Validate([FromBody] ValidacionRecetaRequestDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.CodigoReceta))
            return BadRequest(new { code = "invalid_request", message = "codigoReceta requerido" });

        try
        {
            var baseUrl = _configuration["HospitalApi:BaseUrl"];
            var validatePath = _configuration["HospitalApi:ValidatePath"] ?? "/api/Receta/validar-externa";

            if (string.IsNullOrWhiteSpace(baseUrl))
                return Ok(new { codigoReceta = dto.CodigoReceta, valida = true, origen = "mock" });

            var client = _httpClientFactory.CreateClient("HospitalApi");

            // Construir payload externo
            var payload = JsonSerializer.Serialize(new { codigoReceta = dto.CodigoReceta });
            using var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            // POST externo
            var resp = await client.PostAsync(validatePath, content);

            if (!resp.IsSuccessStatusCode)
                return BadRequest(new { code = "recipe_invalid", message = "Receta no válida o no encontrada" });

            var json = await resp.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { code = "hospital_unavailable", message = "No se pudo contactar al hospital", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { code = "unexpected_error", message = "Error inesperado", detail = ex.Message });
        }
    }
    }
}