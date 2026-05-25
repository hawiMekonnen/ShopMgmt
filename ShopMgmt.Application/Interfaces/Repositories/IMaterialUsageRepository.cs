using System.Threading;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IMaterialUsageRepository
{
    Task<MaterialUsage> AddAsync(MaterialUsage usage);
    Task<List<MaterialUsage>> GetByShopAsync(int shopId);
    Task<IReadOnlyList<MaterialUsage>> GetRecentByShopAsync(int shopId, int take, CancellationToken cancellationToken = default);
    Task<MaterialUsage?> GetByIdAsync(int usageId);
    Task UpdateAsync(MaterialUsage usage);
    Task DeleteAsync(int usageId);
}
