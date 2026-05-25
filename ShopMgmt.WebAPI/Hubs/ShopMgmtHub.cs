using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ShopMgmt.WebAPI.Hubs;

[Authorize]
public class ShopMgmtHub : Hub
{
    // Clients can listen to "NewMaterialAdded" event
    public async Task NotifyNewMaterial(int materialId, string name)
    {
        await Clients.All.SendAsync("NewMaterialAdded", materialId, name);
    }
}
