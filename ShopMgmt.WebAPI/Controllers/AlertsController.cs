using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    // GET /api/alerts
    [HttpGet]
    public async Task<ActionResult<List<AlertDto>>> GetActive()
    {
        var alerts = await _alertService.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    // PATCH /api/alerts/{id}/resolve
    [HttpPatch("{id:int}/resolve")]
    public async Task<ActionResult<AlertDto>> Resolve(int id, [FromBody] ResolveAlertDto dto)
    {
        var result = await _alertService.ResolveAlertAsync(id, dto);
        return Ok(result);
    }

    // POST /api/alerts/check  — manual trigger for dev/testing
    [HttpPost("check")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> TriggerCheck()
    {
        await _alertService.CheckAndCreateLowStockAlertsAsync();
        await _alertService.CheckAndCreateExpiryAlertsAsync();
        return NoContent();
    }
}
