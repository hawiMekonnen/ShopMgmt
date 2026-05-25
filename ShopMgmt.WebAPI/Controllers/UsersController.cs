using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.WebAPI.Authorization;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet("technicians")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> GetTechnicians(
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
    {
        var resolvedShopId = ShopScopeHelper.ResolveShopId(User, shopId)
            ?? throw new BadHttpRequestException("Shop id is required.");
        return Ok(await _userService.GetTechniciansForShopAsync(resolvedShopId, cancellationToken));
    }

    [HttpPost("technicians")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<UserListItemDto>> CreateTechnician(
        [FromBody] CreateTechnicianDto dto,
        CancellationToken cancellationToken)
    {
        var shopId = ShopScopeHelper.ResolveShopId(User, null)
            ?? throw new BadHttpRequestException("Your account must be linked to a shop.");
        var created = await _userService.CreateTechnicianAsync(shopId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetTechnicians), new { shopId }, created);
    }

    [HttpGet("shop-activity")]
    [Authorize(Roles = "ShopManager,Admin")]
    public async Task<ActionResult<ShopActivityDto>> GetShopActivity(
        [FromQuery] int? shopId,
        CancellationToken cancellationToken)
    {
        var resolvedShopId = ShopScopeHelper.ResolveShopId(User, shopId)
            ?? throw new BadHttpRequestException("Shop id is required.");
        return Ok(await _userService.GetShopActivityAsync(resolvedShopId, cancellationToken));
    }
}
