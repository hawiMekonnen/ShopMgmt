using ShopMgmt.Application.Models;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IMaterialRepository
{
    Task<IReadOnlyList<MaterialListRow>> GetAllWithInventoryAsync(int? shopId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialListRow>> SearchAsync(string? partNumber, string? aircraft, string? query, int? shopId, CancellationToken cancellationToken = default);
    Task<Material?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MaterialInventorySnapshot?> GetInventoryAsync(int materialId, int? shopId = null, CancellationToken cancellationToken = default);
    Task<bool> HasUsagesAsync(int materialId, CancellationToken cancellationToken = default);
    Task<bool> HasStockBatchesAsync(int materialId, CancellationToken cancellationToken = default);
    Task<Material> AddAsync(Material material, CancellationToken cancellationToken = default);
    Task UpdateAsync(Material material, CancellationToken cancellationToken = default);
    Task DeleteAsync(Material material, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> PartNumberExistsAsync(string partNumber, int? excludeMaterialId = null, CancellationToken cancellationToken = default);
}
