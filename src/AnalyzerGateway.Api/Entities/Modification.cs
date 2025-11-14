using System;

namespace AnalyzerGateway.Api.Entities
{
    public class Modification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AnalysisId { get; set; } = default!;
        public Analysis? Analysis { get; set; }
        public string Category { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string RefactoringSuggestion { get; set; } = default!;
        public string State { get; set; } = default!;
        public string CssSelector { get; set; } = default!;
        public string Severity { get; set; } = default!;
    }
}
