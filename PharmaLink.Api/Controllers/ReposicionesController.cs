using Microsoft.AspNetCore.Mvc;
using PharmaLink.Api.Models.Dto;
using PharmaLink.Data;
using PharmaLink.Api.Models;
using PharmaLink.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace PharmaLink.Api.Controllers
{
    [ApiController]
    [Route("api/reposiciones")]
    public class ReposicionesController : ControllerBase
    {
        private readonly PharmaLinkContext _context;

        public ReposicionesController(PharmaLinkContext context)
        {
            _context = context;
        }

        // POST: api/reposiciones/pedidos
        [HttpPost("pedidos")]
        public IActionResult RecibirPedido([FromBody] PedidoInsumosDto pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Code = "invalid_request", Message = "Pedido inválido" });
            }

            var resumenItems = new List<object>();
            int totalItems = 0;
            int totalConfirmed = 0;

            foreach (var item in pedido.Items)
            {
                totalItems++;
                // Resolver insumo por Id (si codigo es numérico) o por nombre
                Insumo? insumo = null;
                if (int.TryParse(item.Codigo, out var possibleId))
                {
                    insumo = _context.Insumos.Find(possibleId);
                }

                if (insumo == null)
                {
                    insumo = _context.Insumos.FirstOrDefault(i => i.Nombre.Equals(item.Codigo, StringComparison.OrdinalIgnoreCase));
                }

                int cantidadConfirmada = 0;
                if (insumo != null)
                {
                    cantidadConfirmada = Math.Min(item.CantidadSolicitada, insumo.Stock);
                    totalConfirmed += cantidadConfirmada;
                }

                resumenItems.Add(new
                {
                    Codigo = item.Codigo,
                    CantidadSolicitada = item.CantidadSolicitada,
                    CantidadConfirmada = cantidadConfirmada
                });
            }

            string status;
            if (totalConfirmed == 0) status = "REJECTED";
            else if (totalConfirmed < pedido.Items.Sum(i => i.CantidadSolicitada)) status = "PARTIAL";
            else status = "ACCEPTED";

            var response = new
            {
                PedidoId = pedido.PedidoId,
                Status = status,
                Items = resumenItems
            };

            return Ok(response);
        }

        // POST: api/reposiciones/confirmacion
        [HttpPost("confirmacion")]
        public async Task<IActionResult> ConfirmacionEnvio([FromBody] ConfirmacionEnvioDto confirm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Code = "invalid_request", Message = "Confirmación inválida" });
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var resultados = new List<object>();

                foreach (var item in confirm.Items)
                {
                    Insumo? insumo = null;
                    if (int.TryParse(item.Codigo, out var possibleId))
                    {
                        insumo = await _context.Insumos.FindAsync(possibleId);
                    }

                    if (insumo == null)
                    {
                        insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Nombre.Equals(item.Codigo, StringComparison.OrdinalIgnoreCase));
                    }

                    if (insumo != null)
                    {
                        insumo.Stock += item.CantidadEnviada;
                        resultados.Add(new { Codigo = item.Codigo, CantidadActualizada = insumo.Stock });
                    }
                    else
                    {
                        // Si el insumo no existe, lo creamos mínimamente
                        var nuevo = new Insumo
                        {
                            Nombre = item.Codigo,
                            Presentacion = string.Empty,
                            Stock = item.CantidadEnviada,
                            PrecioUnitario = 0m
                        };
                        _context.Insumos.Add(nuevo);
                        resultados.Add(new { Codigo = item.Codigo, CantidadActualizada = nuevo.Stock });
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var resp = new
                {
                    PedidoId = confirm.PedidoId,
                    FechaEnvio = confirm.FechaEnvio,
                    NumeroRemito = confirm.NumeroRemito,
                    Items = resultados
                };

                return Ok(resp);
            }
            catch
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ErrorResponse { Code = "db_error", Message = "Error actualizando stock durante la confirmación" });
            }
        }
    }
}
