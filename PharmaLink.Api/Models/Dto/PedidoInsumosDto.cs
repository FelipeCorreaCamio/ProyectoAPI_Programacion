namespace PharmaLink.Api.Models.Dto
{
    public class PedidoInsumosDto
    {
        public string PedidoId { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public List<PedidoItemDto> Items { get; set; } = new();
        public ContactoDto Contacto { get; set; } = new();
    }

    public class PedidoItemDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public int CantidadSolicitada { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }

    public class ContactoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}