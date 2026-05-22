using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialRequestsController : ControllerBase
{
    private readonly IMaterialRequestService _requestService;

    public MaterialRequestsController(IMaterialRequestService requestService) => _requestService = requestService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaterialRequestDto>>> List(
        [FromQuery] int? shopId,
        [FromQuery] RequestStatus? status,
        [FromQuery] int? userId,
        CancellationToken cancellationToken)
        => Ok(await _requestService.ListAsync(shopId, status, userId, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialRequestDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _requestService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Technician,ShopManager,Admin")]
    public async Task<ActionResult<MaterialRequestDto>> Submit(
        [FromBody] CreateMaterialRequestDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var created = await _requestService.SubmitAsync(dto, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.RequestId }, created);
    }

    [HttpPatch("{id:int}/approve")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<MaterialRequestDto>> Approve(int id, CancellationToken cancellationToken)
        => Ok(await _requestService.ApproveAsync(id, cancellationToken));

    [HttpPatch("{id:int}/ready")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<MaterialRequestDto>> MarkReady(int id, CancellationToken cancellationToken)
        => Ok(await _requestService.MarkReadyAsync(id, cancellationToken));

    [HttpPatch("{id:int}/issue")]
    [Authorize(Roles = "ShopManager,Technician,Admin")]
    public async Task<ActionResult<MaterialRequestDto>> Issue(
        int id,
        [FromBody] IssueMaterialRequestDto dto,
        CancellationToken cancellationToken)
    {
        var issuedBy = GetUserId();
        return Ok(await _requestService.IssueAsync(id, dto, issuedBy, cancellationToken));
    }

    [HttpPatch("{id:int}/cancel")]
    [Authorize(Roles = "ShopManager,Technician,Admin")]
    public async Task<ActionResult<MaterialRequestDto>> Cancel(
        int id,
        [FromBody] CancelMaterialRequestDto? dto,
        CancellationToken cancellationToken)
        => Ok(await _requestService.CancelAsync(id, dto, cancellationToken));

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
