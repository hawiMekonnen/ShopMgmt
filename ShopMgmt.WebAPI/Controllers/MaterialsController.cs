using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService) => _materialService = materialService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaterialListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _materialService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialDetailDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _materialService.GetByIdAsync(id, cancellationToken));

    [HttpGet("{id:int}/inventory")]
    public async Task<ActionResult<MaterialInventoryDto>> GetInventory(int id, CancellationToken cancellationToken)
        => Ok(await _materialService.GetInventoryAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<MaterialDetailDto>> Create([FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        var created = await _materialService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.MaterialId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MaterialDetailDto>> Update(int id, [FromBody] UpdateMaterialDto dto, CancellationToken cancellationToken)
        => Ok(await _materialService.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _materialService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
