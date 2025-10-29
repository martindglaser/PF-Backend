namespace AnalyzerGateway.Api.DTOs
{
    public record AnalysisRequestDto(
        string Url,
        string Tolerance,   // "high" | "medium" | "low"
        string Language,     // "es" | "en" | ...
        string AnalysisName,
        string UserName
        //List<string> Categories
    );
}
