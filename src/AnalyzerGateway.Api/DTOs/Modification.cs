namespace AnalyzerGateway.Api.DTOs
{
    public record ModificacionDto(Guid Id, Guid AnalisisId, string Devolucion, DateTime CreatedAtUtc);
}
