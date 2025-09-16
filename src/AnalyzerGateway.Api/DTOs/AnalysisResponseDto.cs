using System;
using System.Collections.Generic;

namespace AnalyzerGateway.Api.DTOs
{
    public record AnalysisResponseDto(
        Guid Id,
        string Url,
        string Tolerance,
        string Language,
        string WhatHeSee,
        List<ModificacionDto> Modifications,
        DateTime CreatedAtUtc
    );
}
