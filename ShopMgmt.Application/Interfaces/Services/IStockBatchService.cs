using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IStockBatchService
{
    Task<IReadOnlyList<StockBatchDto>> GetByMaterialIdAsync(int materialId, CancellationToken cancellationToken = default);
    Task<StockBatchDto> ReceiveAsync(int materialId, CreateStockBatchDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int materialId, int batchId, CancellationToken cancellationToken = default);
}
