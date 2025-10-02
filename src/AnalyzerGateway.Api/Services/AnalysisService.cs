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
            var result = await _client.AnalyzeAsync(
                req.Url,
                req.Tolerance.ToLowerInvariant(),
                req.Language.ToLowerInvariant(),
                ct);

            var entity = new Analysis
            {
                Id = result.Cuid,
                Url = req.Url,
                Tolerance = req.Tolerance.ToLowerInvariant(),
                Language = req.Language.ToLowerInvariant(),
                WhatHeSee = result.WhatISee ?? string.Empty,
                NeedsModifications = result.NeedsModification,
                DesktopScreen = result.DesktopScreen ?? string.Empty,
                MobileScreen = result.MobileScreen ?? string.Empty,
            };

            var modifications = new List<Modification>();

            if (result.Modifications is not null)
            {
                foreach (var m in result.Modifications)
                {
                    var mod = new Modification
                    {
                        Id = Guid.NewGuid().ToString(),
                        AnalysisId = entity.Id, // FK
                        Category = m.Category ?? string.Empty,
                        Description = m.Description ?? string.Empty,
                        State = m.State ?? string.Empty,
                        CssSelector = m.CssSelector ?? string.Empty,
                        Severity = m.Severity ?? string.Empty
                    };

                    modifications.Add(mod);
                }
            }

            entity.Modifications = modifications;

            _db.Analysis.Add(entity);
            await _db.SaveChangesAsync(ct);

            var modDtoList = entity.Modifications
                .Select(m => new ModificacionDto(
                    m.Id,
                    m.AnalysisId,
                    m.Category,
                    m.Description,
                    m.State,
                    m.Severity,
                    m.CssSelector))
                .ToList();

            return new AnalysisResponseDto(
                entity.Id,
                entity.Url,
                entity.Tolerance,
                entity.Language,
                entity.WhatHeSee,
                entity.NeedsModifications,
                entity.DesktopScreen,
                entity.MobileScreen,
                modDtoList,
                entity.CreatedAtUtc
            );
        }

        public async Task<AnalysisResponseDto?> GetAll(string id, CancellationToken ct)
        {
            var e = await _db.Analysis
                             .AsNoTracking()
                             .Include(a => a.Modifications)
                             .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (e is null) return null;

            var mods = e.Modifications
                        .Select(m => new ModificacionDto(
                            m.Id,
                            m.AnalysisId,
                            m.Category,
                            m.Description,
                            m.State,
                            m.Severity,
                            m.CssSelector
                        ))
                        .ToList();

            return new AnalysisResponseDto(
                e.Id,
                e.Url,
                e.Tolerance,
                e.Language,
                e.WhatHeSee,
                e.NeedsModifications,
                e.DesktopScreen,
                e.MobileScreen,
                mods,
                e.CreatedAtUtc
            );
        }


        public async Task<List<AnalysisResponseDto>> GetAllPaged(string? url, int page, int pageSize, CancellationToken ct)
        {
            page = page < 1 ? 1 : page;
            pageSize = (pageSize <= 0 || pageSize > 200) ? 20 : pageSize;

            IQueryable<Analysis> q = _db.Analysis.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(url))
                q = q.Where(a => a.Url.Contains(url));

            q = q.OrderByDescending(x => x.CreatedAtUtc);

            var pageItems = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AnalysisResponseDto(
                    a.Id,
                    a.Url,
                    a.Tolerance,
                    a.Language,
                    a.WhatHeSee,
                    a.NeedsModifications,
                    a.DesktopScreen,
                    a.MobileScreen,
                    a.Modifications
                     .Select(m => new ModificacionDto(
                         m.Id,
                         m.AnalysisId,
                         m.Category,
                         m.Description,
                         m.State,
                         m.Severity,
                         m.CssSelector
                     ))
                     .ToList(),
                    a.CreatedAtUtc
                ))
                .ToListAsync(ct);

            return pageItems;
        }
    }
}
