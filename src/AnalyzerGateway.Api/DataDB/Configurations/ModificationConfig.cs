using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyzerGateway.Api.Data.Configurations
{
    public class ModificationConfig : IEntityTypeConfiguration<Modification>
    {
        public void Configure(EntityTypeBuilder<Modification> b)
        {
            b.ToTable("Modification");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .HasMaxLength(200)
             .IsRequired()
             .ValueGeneratedNever();

            b.Property(x => x.AnalysisId)
             .HasMaxLength(200)
             .IsRequired();

            b.Property(x => x.Category)
             .HasMaxLength(100)
             .IsRequired();

            b.Property(x => x.Description)
             .HasMaxLength(4000)
             .IsRequired();

            b.Property(x => x.State)
             .HasMaxLength(50)
             .IsRequired();

            b.Property(x => x.CssSelector)
             .HasMaxLength(1000)
             .IsRequired();
            
            b.Property(x => x.Severity)
             .HasMaxLength(100)
             .IsRequired();

            b.HasIndex(x => x.AnalysisId);

            b.HasOne(m => m.Analysis)
             .WithMany(a => a.Modifications)
             .HasForeignKey(m => m.AnalysisId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
