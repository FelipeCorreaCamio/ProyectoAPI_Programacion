using System.ComponentModel.DataAnnotations;

namespace PharmaLink.Api.Models.Dto
{
    public class PedidoInsumosDto
    {
        [Required]
        public string PedidoId { get; set; } = string.Empty;

        [Required]
        public string HospitalId { get; set; } = string.Empty;

        public DateTime FechaPedido { get; set; }

        [Required]
        [MinLength(1)]
        public List<PedidoItemDto> Items { get; set; } = new();

        [Required]
        public ContactoDto Contacto { get; set; } = new();
    }

    public class PedidoItemDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Presentacion { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CantidadSolicitada { get; set; }

        public string Unidad { get; set; } = string.Empty;

        public string Prioridad { get; set; } = string.Empty;
    }

    public class ContactoDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;
    }
}