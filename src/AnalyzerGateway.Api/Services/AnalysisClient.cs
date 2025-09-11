using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AnalyzerGateway.Api.Services
{
    public class AnalysisClient
    {
        private readonly HttpClient _http;
        private readonly string _endpoint;

        public AnalysisClient(HttpClient http, IConfiguration config)
        {
            _http = http;
            _endpoint = config["AnalysisApi:Endpoint"] ?? "/analyze";
        }

        public record AnalysisInDto(string url, string tolerance, string language);

        public record AnalysisOutDto(
            List<object> modifications,
            bool needsModification,
            string whatISee,
            string analisisId
        );

        public async Task<AnalysisOutDto> AnalyzeAsync(string url, string tolerance, string language, CancellationToken ct)
        {
            var payload = new AnalysisInDto(url, tolerance, language);
            var resp = await _http.PostAsJsonAsync(_endpoint, payload, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            var dto = JsonSerializer.Deserialize<AnalysisOutDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Respuesta de análisis vacía");

            return dto;
        }
    }
}
