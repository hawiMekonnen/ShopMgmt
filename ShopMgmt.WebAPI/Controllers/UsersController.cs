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
        [FromQuery] int? shopId,
        [FromBody] CreateTechnicianDto dto,
        CancellationToken cancellationToken)
    {
        var resolvedShopId = ShopScopeHelper.ResolveShopId(User, shopId)
            ?? throw new BadHttpRequestException("Shop id is required.");
        var created = await _userService.CreateTechnicianAsync(resolvedShopId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetTechnicians), new { shopId = resolvedShopId }, created);
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

    [HttpGet("managers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> GetShopManagers(CancellationToken cancellationToken)
        => Ok(await _userService.GetShopManagersAsync(cancellationToken));

    [HttpPost("managers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserListItemDto>> CreateShopManager(
        [FromBody] CreateShopManagerDto dto,
        CancellationToken cancellationToken)
    {
        var created = await _userService.CreateShopManagerAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetShopManagers), new { id = created.UserId }, created);
    }
}
