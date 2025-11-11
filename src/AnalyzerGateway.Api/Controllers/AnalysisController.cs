using AnalyzerGateway.Api.DTOs;
using AnalyzerGateway.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

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
        public async Task<ActionResult<AnalysisResponseDto>> Create([FromBody] AnalysisRequestDto req,CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Url))
                return BadRequest("Error: url is required");

            if (!Uri.TryCreate(req.Url, UriKind.Absolute, out var uriResult)
                || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest("Error: the URL is not valid");
            }

            if (string.IsNullOrWhiteSpace(req.Tolerance) ||
                !new[] { "high", "medium", "low" }.Contains(req.Tolerance.ToLower()))
                return BadRequest("Error: tolerance must be 'high'|'medium'|'low'");

            var dto = await _service.CreateAnalysis(req, ct);

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnalysisResponseDto>> Get([FromRoute] string id, CancellationToken ct)
        {
            var dto = await _service.GetAll(id, ct);
            return dto is null ? NotFound() : Ok(dto);
        }


        [HttpGet]
        public async Task<ActionResult<PagedResponse<AnalysisResponseDto>>> List(
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default
        )
        {
            var result = await _service.GetAllPaged(filter, page, pageSize, ct);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken ct)
        {
            var deletedId = await _service.DeleteById(id, ct);
            if (deletedId is null) return NotFound();

            return Ok(new
            {
                message = "Analysis successfully deleted",
                deletedId
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll(CancellationToken ct)
        {
            var deletedCount = await _service.DeleteAll(ct);

            if (deletedCount == 0)
                return Ok(new { message = "There was no analysis to remove.", deletedCount });

            return Ok(new
            {
                message = "All analyses were successfully deleted.",
                deletedCount
            });
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] string? filter, CancellationToken ct)
        {
            var (content, fileName) = await _service.ExportExcel(filter, ct);

            const string contentType =
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(content, contentType, fileName);
        }

    }
}