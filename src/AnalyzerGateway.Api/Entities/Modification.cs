using System;

namespace AnalyzerGateway.Api.Entities
{
    public class Modification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AnalysisId { get; set; }
        public Analysis? Analysis { get; set; }
        public string Devolution { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
