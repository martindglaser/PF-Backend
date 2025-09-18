using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyzerGateway.Api.Data.Configurations
{
    public class AnalysisConfig : IEntityTypeConfiguration<Analysis>
    {
        public void Configure(EntityTypeBuilder<Analysis> b)
        {
            b.ToTable("Analysis");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .HasMaxLength(200)
             .IsRequired()
             .ValueGeneratedNever();

            b.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            b.Property(x => x.Tolerance).HasMaxLength(20).IsRequired();
            b.Property(x => x.Language).HasMaxLength(10).IsRequired();
            b.Property(x => x.WhatHeSee).HasMaxLength(4000).IsRequired();

            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.HasIndex(x => x.CreatedAtUtc);
            b.HasIndex(x => x.Url);

            b.HasMany(a => a.Modifications)
             .WithOne(m => m.Analysis)
             .HasForeignKey(m => m.AnalysisId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
