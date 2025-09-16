namespace AnalyzerGateway.Api.DTOs
{
    public record ModificacionDto(Guid Id, Guid AnalysisId, string Devolution, DateTime CreatedAtUtc);
}
