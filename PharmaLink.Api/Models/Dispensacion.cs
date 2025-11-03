using System;

namespace PharmaLink.Api.Models
{
    public class Dispensacion
    {
        public int Id { get; set; }
        public string CodigoReceta { get; set; } = string.Empty;
        public int MedicamentoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaEntrega { get; set; }
        public bool Confirmado { get; set; }
    }
}
