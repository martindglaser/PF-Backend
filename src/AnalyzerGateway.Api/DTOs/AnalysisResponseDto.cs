using System;
using System.Collections.Generic;

namespace AnalyzerGateway.Api.DTOs
{
    public record AnalysisResponseDto(
        Guid Id,
        string Url,
        string Tolerancia,
        string Lenguage,
        string WhatHeSee,
        List<ModificacionDto> Modificaciones,
        DateTime CreatedAtUtc
    );
}
