namespace PharmaLink.Api.Models.Dto
{
    public class ValidacionRecetaRequestDto
    {
        public string CodigoReceta { get; set; } = string.Empty;
    }

    public class ValidacionRecetaResponseDto
    {
        public string CodigoReceta { get; set; } = string.Empty;
        public bool Valid { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaEmision { get; set; }
        public PacienteDto? Paciente { get; set; }
        public List<RecetaMedDto> Medicamentos { get; set; } = new();
    }

    public class PacienteDto { public string Id { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; }
    public class RecetaMedDto { public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; public int Cantidad { get; set; } }
}