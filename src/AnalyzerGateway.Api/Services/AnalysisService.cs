using AnalyzerGateway.Api.Data;
using AnalyzerGateway.Api.DTOs;
using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AnalyzerGateway.Api.Services
{
    public class AnalysisService
    {
        private readonly AppDbContext _db;
        private readonly AnalysisClient _client;

        public AnalysisService(AppDbContext db, AnalysisClient client)
        {
            _db = db;
            _client = client;
        }

        public async Task<AnalisisResponseDto> CrearAnalisisAsync(AnalisisRequestDto req, CancellationToken ct)
        {
            // 1) llamar API de análisis
            var result = await _client.AnalyzeAsync(req.Url, req.Tolerance.ToLower(), req.Language.ToLower(), ct);

            // 2) mapear y guardar
            var entity = new Analisis
            {
                Id = Guid.NewGuid(),
                Url = req.Url,
                Tolerancia = req.Tolerance.ToLower(),
                Lenguage = req.Language.ToLower(),
                WhatHeSee = result.whatISee,                // del campo whatISee
                Devolucion = JsonSerializer.Serialize(result) // json completo
            };

            _db.Analisis.Add(entity);
            await _db.SaveChangesAsync(ct);

            // 3) devolver DTO
            return new AnalisisResponseDto(
                entity.Id, entity.Url, entity.Tolerancia, entity.Lenguage,
                entity.WhatHeSee, entity.Devolucion, entity.CreatedAtUtc
            );
        }

        public async Task<AnalisisResponseDto?> ObtenerAsync(Guid id, CancellationToken ct)
        {
            var e = await _db.Analisis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return e is null ? null
                : new AnalisisResponseDto(e.Id, e.Url, e.Tolerancia, e.Lenguage, e.WhatHeSee, e.Devolucion, e.CreatedAtUtc);
        }

        public async Task<List<AnalisisResponseDto>> ListarAsync(string? url, int page, int pageSize, CancellationToken ct)
        {
            var q = _db.Analisis.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc);
            if (!string.IsNullOrWhiteSpace(url)) q = q.Where(x => x.Url.Contains(url)).OrderByDescending(x => x.CreatedAtUtc);

            return await q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(e => new AnalisisResponseDto(e.Id, e.Url, e.Tolerancia, e.Lenguage, e.WhatHeSee, e.Devolucion, e.CreatedAtUtc))
                .ToListAsync(ct);
        }
    }
}
