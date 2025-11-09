using System.ComponentModel.DataAnnotations;

namespace PharmaLink.Api.Models.Dto
{
    public class ConfirmacionEnvioDto
    {
        [Required]
        public string PedidoId { get; set; } = string.Empty;

        public DateTime FechaEnvio { get; set; }

        [Required]
        [MinLength(1)]
        public List<ConfirmacionItemDto> Items { get; set; } = new();

        public string NumeroRemito { get; set; } = string.Empty;
    }

    public class ConfirmacionItemDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CantidadEnviada { get; set; }
    }
}
