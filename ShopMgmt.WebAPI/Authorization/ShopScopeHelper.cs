using System.Security.Claims;

namespace ShopMgmt.WebAPI.Authorization;

public static class ShopScopeHelper
{
    /// <summary>
    /// Shop managers are always scoped to their assigned shop from the JWT.
    /// Other roles may pass an optional shopId query parameter.
    /// </summary>
    public static int? ResolveShopId(ClaimsPrincipal user, int? shopIdFromQuery)
    {
        if (user.IsInRole("ShopManager"))
        {
            var claim = user.FindFirstValue("shopId");
            if (int.TryParse(claim, out var managerShopId))
                return managerShopId;
        }

        return shopIdFromQuery;
    }
}
