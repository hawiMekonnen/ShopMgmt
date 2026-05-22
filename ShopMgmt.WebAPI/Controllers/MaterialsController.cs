using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService) => _materialService = materialService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaterialListItemDto>>> GetAll(
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
        => Ok(await _materialService.GetAllAsync(shopId, cancellationToken));

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<MaterialListItemDto>>> Search(
        [FromQuery] string? partNumber,
        [FromQuery] string? aircraft,
        [FromQuery] string? q,
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
        => Ok(await _materialService.SearchAsync(partNumber, aircraft, q, shopId, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialDetailDto>> GetById(
        int id,
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
        => Ok(await _materialService.GetByIdAsync(id, shopId, cancellationToken));

    [HttpGet("{id:int}/inventory")]
    public async Task<ActionResult<MaterialInventoryDto>> GetInventory(
        int id,
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
        => Ok(await _materialService.GetInventoryAsync(id, shopId, cancellationToken));

    [HttpPost]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<MaterialDetailDto>> Create([FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        var created = await _materialService.CreateAsync(dto, cancellationToken);
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
}
