using ShopMgmt.Application.DTOS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopMgmt.Application.Interface;

public interface IAlertService
{
    Task<List<AlertDto>> GetActiveAlertsAsync();
    Task<AlertDto> ResolveAlertAsync(int alertId, ResolveAlertDto dto);
    Task CheckAndCreateLowStockAlertsAsync();
    Task CheckAndCreateExpiryAlertsAsync();
}
