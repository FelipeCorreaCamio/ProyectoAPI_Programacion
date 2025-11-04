namespace PharmaLink.Api.Models.Dto
{
    public class ConfirmacionEnvioDto
    {
        public string PedidoId { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public List<ConfirmacionItemDto> Items { get; set; } = new();
        public string NumeroRemito { get; set; } = string.Empty;
    }

    public class ConfirmacionItemDto
    {
        public string Codigo { get; set; } = string.Empty;
        public int CantidadEnviada { get; set; }
    }
}