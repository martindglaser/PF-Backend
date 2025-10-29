using System;
using System.Collections.Generic;

namespace AnalyzerGateway.Api.DTOs
{
    public record AnalysisResponseDto(
        string Id,
        string Url,
        string Tolerance,
        string Language,
        string WhatHeSee,
        bool NeedsModifications,
        string DesktopScreen,
        string MobilepScreen,
        List<ModificacionDto> Modifications,
        DateTime CreatedAtUtc,
        string AnalysisName,
        string UserName
    );
}
