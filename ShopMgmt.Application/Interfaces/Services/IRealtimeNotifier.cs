using ShopMgmt.Application.DTOS;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IRealtimeNotifier
{
    Task NotifyAlertCreatedAsync(AlertDto alert, CancellationToken cancellationToken = default);
    Task NotifyNewMaterialAsync(int materialId, string name, string partNumber, CancellationToken cancellationToken = default);
}
