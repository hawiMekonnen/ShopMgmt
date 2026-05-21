using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ICategoryService _categoryService;

    public DashboardController(IMaterialService materialService, ICategoryService categoryService)
    {
        _materialService = materialService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetAllAsync(cancellationToken);
        var categories = await _categoryService.GetAllAsync(cancellationToken);

        var totalStockValue = materials.Sum(m => m.StockValue);
        var lowStockCount = materials.Count(m => m.OnHand < 10);

        var stats = new DashboardStatsDto
        {
            TotalMaterials = materials.Count,
            TotalCategories = categories.Count,
            TotalStockValue = totalStockValue,
            LowStockCount = lowStockCount
        };

        return Ok(stats);
    }
}
