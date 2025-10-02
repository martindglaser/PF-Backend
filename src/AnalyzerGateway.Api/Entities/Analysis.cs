using System;
using System.Collections.Generic;

namespace AnalyzerGateway.Api.Entities
{
    public class Analysis
    {
        public string Id { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Tolerance { get; set; } = default!;
        public string Language { get; set; } = default!;
        public string WhatHeSee { get; set; } = default!;
        public bool NeedsModifications { get; set; } = default!;
        public string DesktopScreen { get; set; } = default!;
        public string MobileScreen { get; set; } = default!;
        public List<Modification> Modifications { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
