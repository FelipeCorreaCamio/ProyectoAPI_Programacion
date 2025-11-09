using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PharmaLink.Api.Models;
using PharmaLink.Api.Models.Dto;
using PharmaLink.Data;
using PharmaLink.Api.Utils;

namespace PharmaLink.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispensacionesController : ControllerBase
    {
        private readonly PharmaLinkContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DispensacionesController(PharmaLinkContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // POST: api/dispensaciones
        [HttpPost]
        public async Task<IActionResult> PostDispensacion([FromBody] DispensacionRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Code = "invalid_request", Message = "Datos de entrada inválidos" });
            }

            // comprobar que la API del hospital está configurada
            var baseUrl = _configuration["HospitalApi:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                return StatusCode(503, new ErrorResponse { Code = "hospital_api_unconfigured", Message = "API del hospital no configurada" });
            }

            var client = _httpClientFactory.CreateClient("HospitalApi");
            var hospitalApiKey = _configuration["HospitalApi:ApiKey"];
            if (!string.IsNullOrEmpty(hospitalApiKey))
            {
                client.DefaultRequestHeaders.Remove("X-API-KEY");
                client.DefaultRequestHeaders.Add("X-API-KEY", hospitalApiKey);
            }

            // validar receta en la API del hospital
            HttpResponseMessage resp;
            try
            {
                resp = await client.GetAsync($"/recetas/{dto.CodigoReceta}/validate");
            }
            catch
            {
                return StatusCode(503, new ErrorResponse { Code = "hospital_api_error", Message = "No se pudo contactar la API del hospital" });
            }

            if (!resp.IsSuccessStatusCode)
            {
                return StatusCode((int)resp.StatusCode, new ErrorResponse { Code = "hospital_api_error", Message = "Error desde la API del hospital" });
            }

            var content = await resp.Content.ReadAsStringAsync();
            bool recetaValida = false;
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("valid", out var v))
                {
                    recetaValida = v.GetBoolean();
                }
                else if (doc.RootElement.TryGetProperty("estado", out var e))
                {
                    var estado = e.GetString();
                    recetaValida = string.Equals(estado, "ACTIVA", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return StatusCode(502, new ErrorResponse { Code = "hospital_api_unexpected", Message = "Respuesta inesperada de la API del hospital" });
            }

            if (!recetaValida)
            {
                return BadRequest(new ErrorResponse { Code = "invalid_recipe", Message = "La receta no es válida o no está activa" });
            }

            // comprobar medicamento y stock
            var medicamento = await _context.Medicamentos.FindAsync(dto.MedicamentoId);
            if (medicamento == null)
            {
                return NotFound(new ErrorResponse { Code = "not_found", Message = "Medicamento no encontrado" });
            }

            if (medicamento.Stock < dto.Cantidad)
            {
                return BadRequest(new ErrorResponse { Code = "insufficient_stock", Message = "Stock insuficiente" });
            }

            // registrar dispensación y decrementar stock en transacción
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // re-obtener el medicamento dentro de la transacción para evitar TOCTOU
                var medEnTransaccion = await _context.Medicamentos.FindAsync(dto.MedicamentoId);
                if (medEnTransaccion == null)
                {
                    await tx.RollbackAsync();
                    return NotFound(new ErrorResponse { Code = "not_found", Message = "Medicamento no encontrado" });
                }

                if (medEnTransaccion.Stock < dto.Cantidad)
                {
                    await tx.RollbackAsync();
                    return BadRequest(new ErrorResponse { Code = "insufficient_stock", Message = "Stock insuficiente" });
                }

                var dispensacion = new Dispensacion
                {
                    CodigoReceta = dto.CodigoReceta,
                    MedicamentoId = dto.MedicamentoId,
                    Cantidad = dto.Cantidad,
                    FechaEntrega = DateTime.UtcNow,
                    Confirmado = true
                };

                _context.Dispensaciones.Add(dispensacion);
                medEnTransaccion.Stock -= dto.Cantidad;
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var respDto = new DispensacionResponseDto
                {
                    Id = dispensacion.Id,
                    CodigoReceta = dispensacion.CodigoReceta,
                    MedicamentoId = dispensacion.MedicamentoId,
                    Cantidad = dispensacion.Cantidad,
                    FechaEntrega = dispensacion.FechaEntrega,
                    Confirmado = dispensacion.Confirmado
                };

                return CreatedAtAction(nameof(GetDispensacion), new { id = dispensacion.Id }, respDto);
            }
            catch
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ErrorResponse { Code = "db_error", Message = "Error registrando la dispensación" });
            }
        }

        // GET helper to return a dispensacion by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDispensacion(int id)
        {
            var d = await _context.Dispensaciones.FindAsync(id);
            if (d == null) return NotFound();
            return Ok(d);
        }
    }
}
