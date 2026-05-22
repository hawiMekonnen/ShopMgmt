using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interface;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ShopManager,Admin")]
public class MaterialUsagesController : ControllerBase
{
    private readonly IMaterialUsageService _usageService;

    public MaterialUsagesController(IMaterialUsageService usageService)
    {
        _usageService = usageService;
    }

    [HttpGet("shop/{shopId}")]
    public async Task<ActionResult<List<MaterialUsageDto>>> GetUsagesByShop(int shopId)
    {
        var usages = await _usageService.GetUsagesByShopAsync(shopId);
        return Ok(usages);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaterialUsageDto>> GetUsageById(int id)
    {
        var usage = await _usageService.GetUsageByIdAsync(id);
        if (usage == null)
            return NotFound();

        return Ok(usage);
    }

    [HttpPost]
    public async Task<ActionResult<MaterialUsageDto>> RecordUsage([FromBody] CreateMaterialUsageDto createUsageDto)
    {
        var createdUsage = await _usageService.RecordUsageAsync(createUsageDto);
        return CreatedAtAction(nameof(GetUsageById), new { id = createdUsage.UsageId }, createdUsage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUsage(int id, [FromBody] UpdateMaterialUsageDto updateUsageDto)
    {
        try
        {
            await _usageService.UpdateUsageAsync(id, updateUsageDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsage(int id)
    {
        await _usageService.DeleteUsageAsync(id);
        return NoContent();
    }
}
