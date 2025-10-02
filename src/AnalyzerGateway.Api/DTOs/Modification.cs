namespace AnalyzerGateway.Api.DTOs
{
    public record ModificacionDto(
        string Id, 
        string AnalysisId, 
        string Category,
        string Description,
        string State,
        string CssSelector,
        string Severity);
}
