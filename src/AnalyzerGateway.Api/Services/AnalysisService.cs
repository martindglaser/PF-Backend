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

        public async Task<AnalysisResponseDto> CreateAsyncAnalysis(AnalysisRequestDto req, CancellationToken ct)
        {
            // 1) llamar API de análisis
            var result = await _client.AnalyzeAsync(req.Url, req.Tolerance.ToLower(), req.Language.ToLower(), ct);

            // 2) crear la entidad Analisis
            var entity = new Analisis
            {
                Id = Guid.NewGuid(),
                Url = req.Url,
                Tolerancia = req.Tolerance.ToLower(),
                Lenguage = req.Language.ToLower(),
                WhatHeSee = result.whatISee,
            };

            // 3) crear la lista de Modificacion a partir de result.modifications
            var modificaciones = new List<Modificacion>();

            if (result.modifications is not null)
            {
                foreach (var modElem in result.modifications)
                {
                    string texto;

                    try
                    {
                        // modElem es JsonElement
                        if (modElem.ValueKind == JsonValueKind.String)
                        {
                            texto = modElem.GetString() ?? string.Empty;
                        }
                        else if (modElem.ValueKind == JsonValueKind.Object)
                        {
                            // intentar obtener campos comunes
                            if (modElem.TryGetProperty("devolucion", out var prop) && prop.ValueKind == JsonValueKind.String)
                                texto = prop.GetString() ?? JsonSerializer.Serialize(modElem);
                            else if (modElem.TryGetProperty("devolucionText", out prop) && prop.ValueKind == JsonValueKind.String)
                                texto = prop.GetString() ?? JsonSerializer.Serialize(modElem);
                            else if (modElem.TryGetProperty("text", out prop) && prop.ValueKind == JsonValueKind.String)
                                texto = prop.GetString() ?? JsonSerializer.Serialize(modElem);
                            else if (modElem.TryGetProperty("description", out prop) && prop.ValueKind == JsonValueKind.String)
                                texto = prop.GetString() ?? JsonSerializer.Serialize(modElem);
                            else
                                texto = JsonSerializer.Serialize(modElem); // fallback: serializar objeto
                        }
                        else
                        {
                            texto = modElem.ToString() ?? string.Empty;
                        }
                    }
                    catch
                    {
                        texto = JsonSerializer.Serialize(modElem);
                    }

                    var mod = new Modificacion
                    {
                        Id = Guid.NewGuid(),
                        AnalisisId = entity.Id, // FK
                        Devolucion = texto
                    };

                    modificaciones.Add(mod);
                }
            }

            // 4) agregar Analisis + Modificaciones y guardar
            entity.Modificaciones = modificaciones;

            _db.Analisis.Add(entity);

            // EF Core salvará las Modificaciones por la relación 1-N si están en entity.Modificaciones
            await _db.SaveChangesAsync(ct);

            // 5) mapear a DTOs de respuesta
            var modDtoList = entity.Modificaciones.Select(m => new ModificacionDto(m.Id, m.AnalisisId, m.Devolucion, m.CreatedAtUtc)).ToList();

            return new AnalysisResponseDto(
                entity.Id,
                entity.Url,
                entity.Tolerancia,
                entity.Lenguage,
                entity.WhatHeSee,
                modDtoList,
                entity.CreatedAtUtc
            );
        }

        public async Task<AnalysisResponseDto?> ObtenerAsync(Guid id, CancellationToken ct)
        {
            var e = await _db.Analisis
                             .AsNoTracking()
                             .Include(a => a.Modificaciones)   // trae la colección relacionada
                             .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (e is null) return null;

            var mods = e.Modificaciones
                        .OrderBy(m => m.CreatedAtUtc) // orden opcional
                        .Select(m => new ModificacionDto(m.Id, m.AnalisisId, m.Devolucion, m.CreatedAtUtc))
                        .ToList();

            return new AnalysisResponseDto(
                e.Id,
                e.Url,
                e.Tolerancia,
                e.Lenguage,
                e.WhatHeSee,
                mods,
                e.CreatedAtUtc
            );
        }

        public async Task<List<AnalysisResponseDto>> ListarAsync(string? url, int page, int pageSize, CancellationToken ct)
        {
            page = page < 1 ? 1 : page;
            pageSize = (pageSize <= 0 || pageSize > 200) ? 20 : pageSize;

            var q = _db.Analisis.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc);
            if (!string.IsNullOrWhiteSpace(url)) q = (IOrderedQueryable<Analisis>)q.Where(a => a.Url.Contains(url));

            // Proyección: traer análisis y sus modificaciones (las modificaciones vienen en una lista)
            var pageItems = await q.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .Select(a => new AnalysisResponseDto(
                                        a.Id,
                                        a.Url,
                                        a.Tolerancia,
                                        a.Lenguage,
                                        a.WhatHeSee,
                                        a.Modificaciones.OrderBy(m => m.CreatedAtUtc)
                                                       .Select(m => new ModificacionDto(m.Id, m.AnalisisId, m.Devolucion, m.CreatedAtUtc))
                                                       .ToList(),
                                        a.CreatedAtUtc
                                   ))
                                   .ToListAsync(ct);

            return pageItems;
        }
    }
}
