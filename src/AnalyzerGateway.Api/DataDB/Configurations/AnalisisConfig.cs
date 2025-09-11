using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyzerGateway.Api.Data.Configurations
{
    public class AnalisisConfig : IEntityTypeConfiguration<Analisis>
    {
        public void Configure(EntityTypeBuilder<Analisis> b)
        {
            b.ToTable("Analisis");
            b.HasKey(x => x.Id);

            b.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            b.Property(x => x.Tolerancia).HasMaxLength(20).IsRequired();
            b.Property(x => x.Lenguage).HasMaxLength(10).IsRequired();
            b.Property(x => x.WhatHeSee).HasMaxLength(4000).IsRequired();
            b.Property(x => x.Devolucion).HasColumnType("TEXT").IsRequired(); // JSON
            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.HasIndex(x => x.CreatedAtUtc);
            b.HasIndex(x => x.Url);
        }
    }
}
