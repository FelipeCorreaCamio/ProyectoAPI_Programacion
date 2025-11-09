using PharmaLink.Api.Models;

namespace PharmaLink.Data
{
    public static class DataInitializer
    {
        public static void Initialize(PharmaLinkContext context)
        {
            // Solo inicializar si está vacío (para In-Memory DB o BD nueva)
            if (context.Medicamentos.Any() || context.Insumos.Any())
                return;

            // Agregar medicamentos de prueba
            var medicamentos = new List<Medicamento>
            {
                new Medicamento
                {
                    Codigo = "MED-001",
                    Nombre = "Ibuprofeno",
                    Presentacion = "500mg comprimido",
                    Stock = 100,
                    PrecioUnitario = 5.50m
                },
                new Medicamento
                {
                    Codigo = "MED-002",
                    Nombre = "Paracetamol",
                    Presentacion = "1000mg comprimido",
                    Stock = 150,
                    PrecioUnitario = 3.25m
                },
                new Medicamento
                {
                    Codigo = "MED-003",
                    Nombre = "Amoxicilina",
                    Presentacion = "250mg cápsula",
                    Stock = 50,
                    PrecioUnitario = 8.75m
                },
                new Medicamento
                {
                    Codigo = "MED-004",
                    Nombre = "Omeprazol",
                    Presentacion = "20mg comprimido",
                    Stock = 80,
                    PrecioUnitario = 6.00m
                },
                new Medicamento
                {
                    Codigo = "MED-005",
                    Nombre = "Vitamina C",
                    Presentacion = "500mg comprimido",
                    Stock = 200,
                    PrecioUnitario = 2.50m
                }
            };

            context.Medicamentos.AddRange(medicamentos);

            // Agregar insumos de prueba
            var insumos = new List<Insumo>
            {
                new Insumo
                {
                    Codigo = "INS-001",
                    Nombre = "Gasa estéril",
                    Presentacion = "10x10 cm",
                    Stock = 500,
                    PrecioUnitario = 0.75m
                },
                new Insumo
                {
                    Codigo = "INS-002",
                    Nombre = "Algodón",
                    Presentacion = "500g",
                    Stock = 300,
                    PrecioUnitario = 2.00m
                },
                new Insumo
                {
                    Codigo = "INS-003",
                    Nombre = "Alcohol al 70%",
                    Presentacion = "1L",
                    Stock = 100,
                    PrecioUnitario = 4.50m
                },
                new Insumo
                {
                    Codigo = "INS-004",
                    Nombre = "Vendaje elástico",
                    Presentacion = "5cm x 4.5m",
                    Stock = 200,
                    PrecioUnitario = 3.25m
                },
                new Insumo
                {
                    Codigo = "INS-005",
                    Nombre = "Jeringas estériles",
                    Presentacion = "10ml (caja x 100)",
                    Stock = 50,
                    PrecioUnitario = 15.00m
                }
            };

            context.Insumos.AddRange(insumos);
            context.SaveChanges();
        }
    }
}
