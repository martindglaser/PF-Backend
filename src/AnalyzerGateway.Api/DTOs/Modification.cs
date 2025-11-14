namespace AnalyzerGateway.Api.DTOs
{
    public record ModificacionDto(
        string Id, 
        string AnalysisId, 
        string Category,
        string Description,
        string RefactoringSuggestion,
        string State,
        string Severity,
        string CssSelector);
}
