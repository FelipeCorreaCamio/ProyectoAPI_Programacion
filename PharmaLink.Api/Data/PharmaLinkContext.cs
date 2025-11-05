using Microsoft.EntityFrameworkCore;
using PharmaLink.Api.Models;

namespace PharmaLink.Data
{
    public class PharmaLinkContext : DbContext
    {
        public PharmaLinkContext(DbContextOptions<PharmaLinkContext> options)
            : base(options)
        {
        }

        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
    public DbSet<Dispensacion> Dispensaciones { get; set; }
    }
}
