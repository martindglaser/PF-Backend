namespace AnalyzerGateway.Api.DTOs
{
    public record ModificacionDto(Guid Id, string AnalysisId, string Devolution, DateTime CreatedAtUtc);
}
