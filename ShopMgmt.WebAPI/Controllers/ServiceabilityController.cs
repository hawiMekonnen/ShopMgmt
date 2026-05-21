using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/serviceability")]
public class ServiceabilityController : ControllerBase
{
    private readonly IServiceabilityCheckService _service;

    public ServiceabilityController(IServiceabilityCheckService service)
    {
        _service = service;
    }

    // POST: api/serviceability/check
    // Records a new serviceability check (Technician role only)
    [HttpPost("check")]
    [Authorize(Policy = "RequireTechnician")]
    public async Task<ActionResult<ServiceabilityCheckDto>> RecordCheck([FromBody] CreateServiceabilityCheckDto dto)
    {
        var result = await _service.RecordCheckAsync(dto);
        return CreatedAtAction(nameof(GetHistoryByBatch), new { batchId = result.BatchId }, result);
    }

    // GET: api/serviceability/batches/{batchId}/checks
    // Retrieves the check history for a specific StockBatch
    [HttpGet("batches/{batchId}/checks")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ServiceabilityCheckDto>>> GetHistoryByBatch(int batchId)
    {
        var history = await _service.GetHistoryByBatchAsync(batchId);
        return Ok(history);
    }
}
