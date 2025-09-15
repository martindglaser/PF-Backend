using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyzerGateway.Api.Data.Configurations
{
    public class ModificacionConfig : IEntityTypeConfiguration<Modificacion>
    {
        public void Configure(EntityTypeBuilder<Modificacion> b)
        {
            b.ToTable("Modification");
            b.HasKey(x => x.Id);

            b.Property(x => x.Devolucion).HasColumnType("TEXT").IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();

            // Index en FK para consultas
            b.HasIndex(x => x.AnalisisId);
        }
    }
}
