using System.Collections.Generic;
using System.Threading.Tasks;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IAlertRepository
{
    Task<List<Alert>> GetActiveAlertsAsync();
    Task<List<Alert>> GetByMaterialAsync(int materialId);
    Task<Alert?> GetByIdAsync(int alertId);
    Task<Alert> AddAsync(Alert alert);
    Task UpdateAsync(Alert alert);
    Task<bool> ActiveAlertExistsAsync(int materialId, AlertType type);
    Task<bool> ActivePickupAlertExistsForRequestAsync(int requestId);
}
