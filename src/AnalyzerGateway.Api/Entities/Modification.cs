using System;

namespace AnalyzerGateway.Api.Entities
{
    public class Modificacion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AnalisisId { get; set; }
        public Analisis? Analisis { get; set; }
        public string Devolucion { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
