using System;

namespace AnalyzerGateway.Api.Entities
{
    public class Analisis
    {
        public Guid Id { get; set; }                 // ID
        public string Url { get; set; } = default!;  // URL
        public string Tolerancia { get; set; } = default!; // Tolerancia
        public string Lenguage { get; set; } = default!;   // Lenguage (sic: según tu campo)
        public string WhatHeSee { get; set; } = default!;  // whatHeSee (desde whatISee)
        public string Devolucion { get; set; } = default!; // JSON completo de la respuesta
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
