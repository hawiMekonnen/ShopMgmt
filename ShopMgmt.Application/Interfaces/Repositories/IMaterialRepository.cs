using ShopMgmt.Application.Models;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IMaterialRepository
{
    Task<IReadOnlyList<MaterialListRow>> GetAllWithInventoryAsync(CancellationToken cancellationToken = default);
    Task<Material?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MaterialInventorySnapshot?> GetInventoryAsync(int materialId, CancellationToken cancellationToken = default);
    Task<bool> HasUsagesAsync(int materialId, CancellationToken cancellationToken = default);
    Task<bool> HasStockBatchesAsync(int materialId, CancellationToken cancellationToken = default);
    Task<Material> AddAsync(Material material, CancellationToken cancellationToken = default);
    Task UpdateAsync(Material material, CancellationToken cancellationToken = default);
    Task DeleteAsync(Material material, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
