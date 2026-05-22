using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Procurement,Admin")]
public class ProcurementController : ControllerBase
{
    private readonly IProcurementService _procurementService;

    public ProcurementController(IProcurementService procurementService) => _procurementService = procurementService;

    [HttpGet("actions")]
    public async Task<ActionResult<IReadOnlyList<ProcurementActionDto>>> GetActions(
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
        => Ok(await _procurementService.GetActionsAsync(shopId, cancellationToken));

    [HttpPatch("materials/{materialId:int}/reorder")]
    public async Task<IActionResult> MarkReorder(
        int materialId,
        [FromBody] MarkReorderDto dto,
        CancellationToken cancellationToken)
    {
        await _procurementService.MarkReorderAsync(materialId, dto, cancellationToken);
        return NoContent();
    }
}
