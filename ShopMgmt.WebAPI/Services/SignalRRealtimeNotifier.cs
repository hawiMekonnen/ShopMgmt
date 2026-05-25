using Microsoft.AspNetCore.SignalR;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.WebAPI.Hubs;

namespace ShopMgmt.WebAPI.Services;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ShopMgmtHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<ShopMgmtHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyAlertCreatedAsync(AlertDto alert, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("AlertCreated", alert, cancellationToken);

    public Task NotifyNewMaterialAsync(int materialId, string name, string partNumber, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("NewMaterialAdded", new { materialId, name, partNumber }, cancellationToken);
}
