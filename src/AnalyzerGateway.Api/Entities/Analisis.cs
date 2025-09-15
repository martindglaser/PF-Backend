using System;
using System.Collections.Generic;

namespace AnalyzerGateway.Api.Entities
{
    public class Analisis
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = default!;
        public string Tolerancia { get; set; } = default!;
        public string Lenguage { get; set; } = default!;
        public string WhatHeSee { get; set; } = default!;
        public List<Modificacion> Modificaciones { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
