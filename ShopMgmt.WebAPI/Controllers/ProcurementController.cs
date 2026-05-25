using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.WebAPI.Authorization;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Procurement,Admin,Technician")]
public class ProcurementController : ControllerBase
{
    private readonly IProcurementService _procurementService;
    private readonly IAlertService _alertService;

    public ProcurementController(IProcurementService procurementService, IAlertService alertService)
    {
        _procurementService = procurementService;
        _alertService = alertService;
    }

    [HttpGet("actions")]
    public async Task<ActionResult<IReadOnlyList<ProcurementActionDto>>> GetActions(
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
        => Ok(await _procurementService.GetActionsAsync(shopId, cancellationToken));

    [HttpPost("materials")]
    [Authorize(Policy = "RequireProcurement")]
    public async Task<ActionResult<MaterialDto>> AddMaterial([FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        var created = await _procurementService.AddMaterialAsync(dto, cancellationToken);
        var userId = UserClaimsHelper.GetUserId(User);
        await _alertService.CreateNewMaterialAlertAsync(
            created.MaterialId,
            created.Name,
            created.PartNumber,
            userId,
            cancellationToken);
        return Created(string.Empty, created);
    }

    [HttpPatch("materials/{materialId:int}/reorder")]
    public async Task<IActionResult> MarkReorder(
        int materialId,
        [FromBody] MarkReorderDto dto,
        CancellationToken cancellationToken)
    {
        await _procurementService.MarkReorderAsync(materialId, dto, cancellationToken);
        return NoContent();
    }

    [HttpPatch("requests/{requestId:int}/ready")]
    public async Task<ActionResult<MaterialRequestDto>> MarkReady(
        int requestId,
        [FromBody] MarkReadyDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _procurementService.MarkReadyAsync(requestId, dto.Notes, cancellationToken);
        return Ok(updated);
    }
}
