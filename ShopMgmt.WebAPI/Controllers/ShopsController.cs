using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interface;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopsController : ControllerBase
{
    private readonly IShopService _shopService;

    public ShopsController(IShopService shopService)
    {
        _shopService = shopService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShopDto>>> GetAllShops()
    {
        var shops = await _shopService.GetAllShopsAsync();
        return Ok(shops);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShopDto>> GetShopById(int id)
    {
        var shop = await _shopService.GetShopByIdAsync(id);
        if (shop == null)
            return NotFound();

        return Ok(shop);
    }

    [HttpPost]
    public async Task<ActionResult<ShopDto>> CreateShop([FromBody] CreateShopDto createShopDto)
    {
        var createdShop = await _shopService.CreateShopAsync(createShopDto);
        return CreatedAtAction(nameof(GetShopById), new { id = createdShop.ShopId }, createdShop);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShop(int id, [FromBody] UpdateShopDto updateShopDto)
    {
        try
        {
            await _shopService.UpdateShopAsync(id, updateShopDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Assuming exception means not found for now. 
            // In a real app we'd use custom exceptions or better handling.
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShop(int id)
    {
        await _shopService.DeleteShopAsync(id);
        return NoContent();
    }
}
