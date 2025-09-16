using AnalyzerGateway.Api.Data;
using AnalyzerGateway.Api.DTOs;
using AnalyzerGateway.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<AnalysisResponseDto> CreateAnalysis(AnalysisRequestDto req, CancellationToken ct)
        {
            // 1) llamar API de análisis
            var result = await _client.AnalyzeAsync(req.Url, req.Tolerance.ToLower(), req.Language.ToLower(), ct);

            // 2) crear la entidad Analysis
            var entity = new Analysis
            {
                Id = Guid.NewGuid(),
                Url = req.Url,
                Tolerance = req.Tolerance.ToLower(),
                Language = req.Language.ToLower(),
                WhatHeSee = result.whatISee,
            };

            // 3) crear la lista de Modificacion a partir de result.modifications
            var modifications = new List<Modification>();

            if (result.modifications is not null)
            {
                foreach (var modElem in result.modifications)
                {
                    string text;

                    try
                    {
                        if (modElem.ValueKind == JsonValueKind.String)
                        {
                            text = modElem.GetString() ?? string.Empty;

                        }
                        else
                        {
                            text = modElem.ToString() ?? string.Empty;
                        }
                    }
                    catch
                    {
                        text = JsonSerializer.Serialize(modElem);
                    }

                    var mod = new Modification
                    {
                        Id = Guid.NewGuid(),
                        AnalysisId = entity.Id, // FK
                        Devolution = text
                    };

                    modifications.Add(mod);
                }
            }

            // 4) agregar Analisis + Modificaciones y guardar
            entity.Modifications = modifications;

            _db.Analysis.Add(entity);

            // EF Core salvará las Modificaciones por la relación 1-N si están en entity.Modificaciones
            await _db.SaveChangesAsync(ct);

            // 5) mapear a DTOs de respuesta
            var modDtoList = entity.Modifications.Select(m => new ModificacionDto(m.Id, m.AnalysisId, m.Devolution, m.CreatedAtUtc)).ToList();

            return new AnalysisResponseDto(
                entity.Id,
                entity.Url,
                entity.Tolerance,
                entity.Language,
                entity.WhatHeSee,
                modDtoList,
                entity.CreatedAtUtc
            );
        }

        public async Task<AnalysisResponseDto?> GetAll(Guid id, CancellationToken ct)
        {
            var e = await _db.Analysis
                             .AsNoTracking()
                             .Include(a => a.Modifications)
                             .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (e is null) return null;

            var mods = e.Modifications
                        .OrderBy(m => m.CreatedAtUtc)
                        .Select(m => new ModificacionDto(m.Id, m.AnalysisId, m.Devolution, m.CreatedAtUtc))
                        .ToList();

            return new AnalysisResponseDto(
                e.Id,
                e.Url,
                e.Tolerance,
                e.Language,
                e.WhatHeSee,
                mods,
                e.CreatedAtUtc
            );
        }

        public async Task<List<AnalysisResponseDto>> GetAllPaged(string? url, int page, int pageSize, CancellationToken ct)
        {
            page = page < 1 ? 1 : page;
            pageSize = (pageSize <= 0 || pageSize > 200) ? 20 : pageSize;

            var q = _db.Analysis.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc);
            if (!string.IsNullOrWhiteSpace(url)) q = (IOrderedQueryable<Analysis>)q.Where(a => a.Url.Contains(url));

            // Proyección: traer análisis y sus modificaciones (las modificaciones vienen en una lista)
            var pageItems = await q.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .Select(a => new AnalysisResponseDto(
                                        a.Id,
                                        a.Url,
                                        a.Tolerance,
                                        a.Language,
                                        a.WhatHeSee,
                                        a.Modifications.OrderBy(m => m.CreatedAtUtc)
                                                       .Select(m => new ModificacionDto(m.Id, m.AnalysisId, m.Devolution, m.CreatedAtUtc))
                                                       .ToList(),
                                        a.CreatedAtUtc
                                   ))
                                   .ToListAsync(ct);

            return pageItems;
        }
    }
}
