using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.WebAPI.Authorization;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly IAlertService _alertService;

    public MaterialsController(IMaterialService materialService, IAlertService alertService)
    {
        _materialService = materialService;
        _alertService = alertService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,ShopManager,Procurement,Technician")]
    public async Task<ActionResult<IReadOnlyList<MaterialListItemDto>>> GetAll(
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
    {
        var scopedShopId = ShopScopeHelper.ResolveShopId(User, shopId);
        var technicianCatalog = User.IsInRole("Technician");
        return Ok(await _materialService.GetAllAsync(scopedShopId, technicianCatalog, cancellationToken));
    }

    [HttpGet("search")]
    [Authorize(Roles = "Technician,ShopManager,Admin")]
    public async Task<ActionResult<IReadOnlyList<MaterialListItemDto>>> Search(
        [FromQuery] string? partNumber,
        [FromQuery] string? aircraft,
        [FromQuery] string? q,
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
    {
        var scopedShopId = ShopScopeHelper.ResolveShopId(User, shopId);
        var technicianCatalog = User.IsInRole("Technician");
        return Ok(await _materialService.SearchAsync(partNumber, aircraft, q, scopedShopId, technicianCatalog, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,ShopManager,Procurement,Technician")]
    public async Task<ActionResult<MaterialDetailDto>> GetById(
        int id,
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
    {
        var scopedShopId = ShopScopeHelper.ResolveShopId(User, shopId);
        return Ok(await _materialService.GetByIdAsync(id, scopedShopId, cancellationToken));
    }

    [HttpGet("{id:int}/inventory")]
    [Authorize(Roles = "Admin,ShopManager,Procurement,Technician")]
    public async Task<ActionResult<MaterialInventoryDto>> GetInventory(
        int id,
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
    {
        var scopedShopId = ShopScopeHelper.ResolveShopId(User, shopId);
        return Ok(await _materialService.GetInventoryAsync(id, scopedShopId, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<ActionResult<MaterialDetailDto>> Create([FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        var created = await _materialService.CreateAsync(dto, cancellationToken);
        var userId = UserClaimsHelper.GetUserId(User);
        await _alertService.CreateNewMaterialAlertAsync(
            created.MaterialId,
            created.Name,
            created.PartNumber,
            userId,
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.MaterialId }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<MaterialDetailDto>> Update(int id, [FromBody] UpdateMaterialDto dto, CancellationToken cancellationToken)
        => Ok(await _materialService.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _materialService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/technician-visibility")]
    [Authorize(Roles = "Procurement,Admin")]
    public async Task<IActionResult> SetTechnicianVisibility(
        int id,
        [FromBody] SetMaterialTechnicianVisibilityDto dto,
        CancellationToken cancellationToken)
    {
        await _materialService.SetTechnicianVisibilityAsync(id, dto.HiddenFromTechnicians, cancellationToken);
        return NoContent();
    }
}
