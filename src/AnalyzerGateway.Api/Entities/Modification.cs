using System;

namespace AnalyzerGateway.Api.Entities
{
    public class Modification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AnalysisId { get; set; } = default!;
        public Analysis? Analysis { get; set; }
        public string Devolution { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
