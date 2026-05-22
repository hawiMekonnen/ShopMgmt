using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/materials/{materialId:int}/batches")]
[Authorize]
public class StockBatchesController : ControllerBase
{
    private readonly IStockBatchService _stockBatchService;

    public StockBatchesController(IStockBatchService stockBatchService) => _stockBatchService = stockBatchService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockBatchDto>>> GetByMaterial(int materialId, CancellationToken cancellationToken)
        => Ok(await _stockBatchService.GetByMaterialIdAsync(materialId, cancellationToken));

    [HttpPost]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<StockBatchDto>> Receive(int materialId, [FromBody] CreateStockBatchDto dto, CancellationToken cancellationToken)
    {
        var created = await _stockBatchService.ReceiveAsync(materialId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetByMaterial), new { materialId }, created);
    }

    [HttpDelete("{batchId:int}")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<IActionResult> Delete(int materialId, int batchId, CancellationToken cancellationToken)
    {
        await _stockBatchService.DeleteAsync(materialId, batchId, cancellationToken);
        return NoContent();
    }
}
