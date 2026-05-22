using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Technician,ShopManager,Admin")]
public class MaterialReturnsController : ControllerBase
{
    private readonly IMaterialReturnService _returnService;

    public MaterialReturnsController(IMaterialReturnService returnService) => _returnService = returnService;

    [HttpPost]
    public async Task<ActionResult<MaterialReturnDto>> RecordReturn(
        [FromBody] CreateMaterialReturnDto dto,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _returnService.RecordReturnAsync(dto, userId, cancellationToken);
        return Ok(result);
    }
}
