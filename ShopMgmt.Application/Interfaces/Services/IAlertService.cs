using ShopMgmt.Application.DTOS;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IAlertService
{
    Task<List<AlertDto>> GetActiveAlertsAsync();
    Task<AlertDto> ResolveAlertAsync(int alertId, ResolveAlertDto dto);
    Task CheckAndCreateLowStockAlertsAsync();
    Task CheckAndCreateExpiryAlertsAsync();
    Task<AlertDto?> CreateNewMaterialAlertAsync(int materialId, string materialName, string partNumber, int createdBy, CancellationToken cancellationToken = default);
    Task<AlertDto?> CreatePickupReadyAlertAsync(int materialId, int requestId, decimal quantity, int createdBy, CancellationToken cancellationToken = default);
}
