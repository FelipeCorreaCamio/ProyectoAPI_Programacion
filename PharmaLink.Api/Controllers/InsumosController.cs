using Microsoft.AspNetCore.Mvc;
using PharmaLink.Data;
using PharmaLink.Api.Models;

namespace PharmaLink.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsumosController : ControllerBase
    {
        private readonly PharmaLinkContext _context;

        public InsumosController(PharmaLinkContext context)
        {
            _context = context;
        }

    // Obtener lista de insumos (ruta: api/insumos) — básico
        [HttpGet]
        public IActionResult GetInsumos()
        {
            var insumos = _context.Insumos.ToList();
            return Ok(insumos);
        }

    // Crear un insumo (ruta: api/insumos) — lo guardo tal cual
        [HttpPost]
        public IActionResult PostInsumo(Insumo insumo)
        {
            _context.Insumos.Add(insumo);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetInsumos), new { id = insumo.Id }, insumo);
        }

    // Actualizar insumo por id (ruta: api/insumos/{id}) — actualizo los campos principales
        [HttpPut("{id}")]
        public IActionResult PutInsumo(int id, Insumo insumoActualizado)
        {
            var insumo = _context.Insumos.Find(id);
            if (insumo == null) return NotFound();

            insumo.Nombre = insumoActualizado.Nombre;
            insumo.Presentacion = insumoActualizado.Presentacion;
            insumo.Stock = insumoActualizado.Stock;
            insumo.PrecioUnitario = insumoActualizado.PrecioUnitario;

            _context.SaveChanges();
            return Ok(insumo);
        }
    }
}
