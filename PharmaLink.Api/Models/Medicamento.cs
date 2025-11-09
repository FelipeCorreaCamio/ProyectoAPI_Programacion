namespace PharmaLink.Api.Models
{
    public class Medicamento
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}