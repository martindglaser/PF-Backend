namespace AnalyzerGateway.Api.DTOs
{
    public record AnalisisRequestDto(
        string Url,
        string Tolerance,   // "high" | "medium" | "low"
        string Language     // "es" | "en" | ...
    );
}
