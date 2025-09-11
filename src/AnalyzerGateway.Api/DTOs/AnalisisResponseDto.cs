using System;

namespace AnalyzerGateway.Api.DTOs
{
    public record AnalisisResponseDto(
        Guid Id,
        string Url,
        string Tolerancia,
        string Lenguage,
        string WhatHeSee,
        string Devolucion,
        DateTime CreatedAtUtc
    );
}
