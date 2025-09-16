using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnalyzerGateway.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Analysis> Analisis => Set<Analysis>();
        public DbSet<Modification> Modificaciones => Set<Modification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
