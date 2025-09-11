using AnalyzerGateway.Api.DTOs;
using AnalyzerGateway.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyzerGateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly AnalysisService _service;

        public AnalysisController(AnalysisService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<AnalisisResponseDto>> Create([FromBody] AnalisisRequestDto req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Url))
                return BadRequest("url requerida");

            if (string.IsNullOrWhiteSpace(req.Tolerance) ||
                !new[] { "high", "medium", "low" }.Contains(req.Tolerance.ToLower()))
                return BadRequest("tolerance debe ser 'high'|'medium'|'low'");

            var dto = await _service.CrearAnalisisAsync(req, ct);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AnalisisResponseDto>> Get([FromRoute] Guid id, CancellationToken ct)
        {
            var dto = await _service.ObtenerAsync(id, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpGet]
        public async Task<ActionResult<List<AnalisisResponseDto>>> List(
            [FromQuery] string? url, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is <= 0 or > 200 ? 20 : pageSize;
            var lista = await _service.ListarAsync(url, page, pageSize, ct);
            return Ok(lista);
        }
    }
}