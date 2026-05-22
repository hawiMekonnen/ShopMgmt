using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IStockBatchRepository
{
    Task<IReadOnlyList<StockBatch>> GetByMaterialIdAsync(int materialId, CancellationToken cancellationToken = default);
    Task<StockBatch?> GetByIdAsync(int batchId, CancellationToken cancellationToken = default);
    Task<StockBatch> AddAsync(StockBatch batch, CancellationToken cancellationToken = default);
    Task DeleteAsync(StockBatch batch, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockBatch>> GetExpiringBeforeAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockBatch>> GetQuarantinedAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(StockBatch batch, CancellationToken cancellationToken = default);
}
