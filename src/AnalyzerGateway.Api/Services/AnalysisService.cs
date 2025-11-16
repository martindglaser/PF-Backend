using AnalyzerGateway.Api.Data;
using AnalyzerGateway.Api.DTOs;
using AnalyzerGateway.Api.Entities;
using ClosedXML.Excel;
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
                req.Categories,
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
                AnalysisName = req.AnalysisName,
                UserName = req.UserName         
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
                        RefactoringSuggestion = m.RefactoringSuggestion ?? string.Empty,
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
                    m.RefactoringSuggestion,
                    m.State,
                    m.Severity,
                    m.CssSelector
                    ))
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
                entity.CreatedAtUtc,
                entity.AnalysisName, // Nuevo
                entity.UserName      // Nuevo
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
                            m.RefactoringSuggestion,
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
                e.CreatedAtUtc,
                e.AnalysisName, 
                e.UserName      
            );
        }

        public async Task<PagedResponse<AnalysisResponseDto>> GetAllPaged(string? filter, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct)
        {
            page = page < 1 ? 1 : page;
            pageSize = (pageSize <= 0 || pageSize > 200) ? 20 : pageSize;

            IQueryable<Analysis> q = _db.Analysis.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var lower = filter.ToLower();
                q = q.Where(a =>
                    (a.Url ?? "").ToLower().Contains(lower) ||
                    (a.UserName ?? "").ToLower().Contains(lower) ||
                    (a.AnalysisName ?? "").ToLower().Contains(lower)
                );
            }

            if (from.HasValue)
                q = q.Where(a => a.CreatedAtUtc >= from.Value);

            if (to.HasValue)
                q = q.Where(a => a.CreatedAtUtc < to.Value);

            var totalItems = await q.CountAsync(ct);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

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
                         m.RefactoringSuggestion,
                         m.State,
                         m.Severity,
                         m.CssSelector
                     ))
                     .ToList(),
                    a.CreatedAtUtc,
                    a.AnalysisName,
                    a.UserName
                ))
                .ToListAsync(ct);

            return new PagedResponse<AnalysisResponseDto>
            {
                Items = pageItems,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }
        public async Task<string?> DeleteById(string id, CancellationToken ct)
        {
            var entity = await _db.Analysis.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (entity is null) return null;

            _db.Analysis.Remove(entity);
            await _db.SaveChangesAsync(ct);

            return entity.Id;
        }
        public async Task<int> DeleteAll(CancellationToken ct)
        {
            var count = await _db.Analysis.CountAsync(ct);
            if (count == 0) return 0;

            _db.Analysis.RemoveRange(_db.Analysis);
            await _db.SaveChangesAsync(ct);

            return count;
        }
        public async Task<(byte[] Content, string FileName)> ExportExcel(string? filter, DateTime? from, DateTime? to, CancellationToken ct)
        {
            IQueryable<Analysis> q = _db.Analysis
                .AsNoTracking()
                .Include(a => a.Modifications);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var lower = filter.ToLower();
                q = q.Where(a =>
                    (a.Url ?? "").ToLower().Contains(lower) ||
                    (a.UserName ?? "").ToLower().Contains(lower) ||
                    (a.AnalysisName ?? "").ToLower().Contains(lower)
                );
            }

            if (from.HasValue)
                q = q.Where(a => a.CreatedAtUtc >= from.Value);

            if (to.HasValue)
                q = q.Where(a => a.CreatedAtUtc <= to.Value);

            var items = await q
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync(ct);

            using var wb = new XLWorkbook();

            var wsA = wb.Worksheets.Add("Analyses");
            var r = 1;
            wsA.Cell(r, 1).Value = "Id";
            wsA.Cell(r, 2).Value = "Url";
            wsA.Cell(r, 3).Value = "Tolerance";
            wsA.Cell(r, 4).Value = "Language";
            wsA.Cell(r, 5).Value = "NeedsModifications";
            wsA.Cell(r, 6).Value = "DesktopScreen";
            wsA.Cell(r, 7).Value = "MobileScreen";
            wsA.Cell(r, 8).Value = "CreatedAtUtc";
            wsA.Cell(r, 9).Value = "AnalysisName";
            wsA.Cell(r, 10).Value = "UserName";
            wsA.Cell(r, 11).Value = "ModificationsCount";
            wsA.Range(r, 1, r, 11).Style.Font.Bold = true;

            foreach (var a in items)
            {
                r++;
                wsA.Cell(r, 1).Value = a.Id;
                wsA.Cell(r, 2).Value = a.Url;
                wsA.Cell(r, 3).Value = a.Tolerance;
                wsA.Cell(r, 4).Value = a.Language;
                wsA.Cell(r, 5).Value = a.NeedsModifications;
                wsA.Cell(r, 6).Value = a.DesktopScreen;
                wsA.Cell(r, 7).Value = a.MobileScreen;
                wsA.Cell(r, 8).Value = a.CreatedAtUtc;
                wsA.Cell(r, 8).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss";
                wsA.Cell(r, 9).Value = a.AnalysisName;
                wsA.Cell(r, 10).Value = a.UserName;
                wsA.Cell(r, 11).Value = a.Modifications?.Count ?? 0;
            }

            wsA.Columns().AdjustToContents();

            var wsM = wb.Worksheets.Add("Modifications");
            r = 1;
            wsM.Cell(r, 1).Value = "AnalysisId";
            wsM.Cell(r, 2).Value = "Category";
            wsM.Cell(r, 3).Value = "Description";
            wsM.Cell(r, 3).Value = "RefactoringSuggestion";
            wsM.Cell(r, 5).Value = "State";
            wsM.Cell(r, 6).Value = "Severity";
            wsM.Cell(r, 7).Value = "CssSelector";
            wsM.Cell(r, 8).Value = "CreatedAtUtc";
            wsM.Range(r, 1, r, 8).Style.Font.Bold = true;

            foreach (var a in items)
            {
                if (a.Modifications == null) continue;

                foreach (var m in a.Modifications.OrderBy(x => x.Severity))
                {
                    r++;
                    wsM.Cell(r, 1).Value = a.Id;
                    wsM.Cell(r, 2).Value = m.Category;
                    wsM.Cell(r, 3).Value = m.Description;
                    wsM.Cell(r, 4).Value = m.RefactoringSuggestion;
                    wsM.Cell(r, 5).Value = m.State;
                    wsM.Cell(r, 6).Value = m.Severity;
                    wsM.Cell(r, 7).Value = m.CssSelector;
                }
            }

            wsM.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            var bytes = ms.ToArray();

            var fileName = $"analysis_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            return (bytes, fileName);
        }

    }
}
