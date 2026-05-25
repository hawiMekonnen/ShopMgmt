using System.Security.Claims;

namespace ShopMgmt.WebAPI.Authorization;

public static class UserClaimsHelper
{
    public static int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
