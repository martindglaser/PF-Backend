using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyzerGateway.Api.Data.Configurations
{
    public class ModificacionConfig : IEntityTypeConfiguration<Modification>
    {
        public void Configure(EntityTypeBuilder<Modification> b)
        {
            b.ToTable("Modification");
            b.HasKey(x => x.Id);

            b.Property(x => x.Devolution).HasColumnType("TEXT").IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();

            b.HasIndex(x => x.AnalysisId);
        }
    }
}
