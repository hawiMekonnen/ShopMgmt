using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    // GET /api/auditlogs?entity=Material&performedBy=1&from=2025-01-01&page=1&pageSize=50
    [HttpGet]
    public async Task<ActionResult<AuditLogPagedDto>> GetLogs(
        [FromQuery] string? entity,
        [FromQuery] int? performedBy,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _auditLogService.GetLogsAsync(entity, performedBy, from, to, page, pageSize);
        return Ok(result);
    }
}
