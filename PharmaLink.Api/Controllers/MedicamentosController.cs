using Microsoft.AspNetCore.Mvc;
using PharmaLink.Data;
using PharmaLink.Api.Models;

namespace PharmaLink.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentosController : ControllerBase
    {
        private readonly PharmaLinkContext _context;

        public MedicamentosController(PharmaLinkContext context)
        {
            _context = context;
        }

    // Obtener lista de medicamentos (ruta: api/medicamentos) — método simple, sin validaciones extra
        [HttpGet]
        public IActionResult GetMedicamentos()
        {
            var medicamentos = _context.Medicamentos.ToList();
            return Ok(medicamentos);
        }

    // Crear un medicamento (ruta: api/medicamentos) — guardo lo que viene
        [HttpPost]
        public IActionResult PostMedicamento(Medicamento medicamento)
        {
            _context.Medicamentos.Add(medicamento);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMedicamentos), new { id = medicamento.Id }, medicamento);
        }

    // Actualizar medicamento por id (ruta: api/medicamentos/{id}) — actualizo campos principales
        [HttpPut("{id}")]
        public IActionResult PutMedicamento(int id, Medicamento medicamentoActualizado)
        {
            var medicamento = _context.Medicamentos.Find(id);
            if (medicamento == null) return NotFound();

            medicamento.Nombre = medicamentoActualizado.Nombre;
            medicamento.Presentacion = medicamentoActualizado.Presentacion;
            medicamento.Stock = medicamentoActualizado.Stock;
            medicamento.PrecioUnitario = medicamentoActualizado.PrecioUnitario;

            _context.SaveChanges();
            return Ok(medicamento);
        }
    }
}
