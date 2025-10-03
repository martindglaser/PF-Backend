using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnalyzerGateway.Api.Services
{
    public class AnalysisClient
    {
        private readonly HttpClient _http;
        private readonly string _endpoint;

        public AnalysisClient(HttpClient http, IConfiguration config)
        {
            _http = http;
            _http.BaseAddress = new Uri(config["AnalysisApi:BaseUrl"]!);

            _http.Timeout = Timeout.InfiniteTimeSpan;

            _endpoint = config["AnalysisApi:Endpoint"] ?? "/analyze";
        }

        public record AnalysisInDto(string url, string tolerance, string language);

        public record ModificationOutDto(
            [property: JsonPropertyName("categoria")] string? Category,
            [property: JsonPropertyName("descripcion")] string? Description,
            [property: JsonPropertyName("estado")] string? State,
            [property: JsonPropertyName("selector_css")] string? CssSelector,
            [property: JsonPropertyName("severidad")] string? Severity
        );

        public record AnalysisOutDto(
            [property: JsonPropertyName("cuid")] string Cuid,
            [property: JsonPropertyName("modifications")] ModificationOutDto[]? Modifications,
            [property: JsonPropertyName("needsModification")] bool NeedsModification,
            [property: JsonPropertyName("mobile_screenshot")] string? MobileScreen,
            [property: JsonPropertyName("desktop_screenshot")] string? DesktopScreen,
            [property: JsonPropertyName("whatISee")] string? WhatISee,
            [property: JsonPropertyName("analysisId")] string? AnalysisId
        );

        public async Task<AnalysisOutDto> AnalyzeAsync(string url, string tolerance, string language, CancellationToken ct)
        {
            var payload = new AnalysisInDto(url, tolerance, language);

            using var req = new HttpRequestMessage(HttpMethod.Post, _endpoint)
            {
                Content = JsonContent.Create(payload)
            };

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromMinutes(8));

            using var resp = await _http.SendAsync(
                req,
                HttpCompletionOption.ResponseHeadersRead,
                cts.Token);

            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(cts.Token);

            var dto = await JsonSerializer.DeserializeAsync<AnalysisOutDto>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cts.Token) ?? throw new InvalidOperationException("Error: empty response");

            return dto;
        }
    }
}
