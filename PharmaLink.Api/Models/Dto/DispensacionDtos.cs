using System.ComponentModel.DataAnnotations;

namespace PharmaLink.Api.Models.Dto
{
    public class DispensacionRequestDto
    {
        [Required]
        public string CodigoReceta { get; set; } = string.Empty;

        [Required]
        public int MedicamentoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }

    public class DispensacionResponseDto
    {
        public int Id { get; set; }
        public string CodigoReceta { get; set; } = string.Empty;
        public int MedicamentoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaEntrega { get; set; }
        public bool Confirmado { get; set; }
    }
}
