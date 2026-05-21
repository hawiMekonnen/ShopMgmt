using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class StockBatchRepository : IStockBatchRepository
{
    private readonly AppDbContext _context;

    public StockBatchRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<StockBatch>> GetByMaterialIdAsync(int materialId, CancellationToken cancellationToken = default)
        => await _context.StockBatches
            .Where(b => b.MaterialId == materialId)
            .OrderByDescending(b => b.ReceivedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<StockBatch?> GetByIdAsync(int batchId, CancellationToken cancellationToken = default)
        => await _context.StockBatches.FindAsync([batchId], cancellationToken);

    public async Task<StockBatch> AddAsync(StockBatch batch, CancellationToken cancellationToken = default)
    {
        _context.StockBatches.Add(batch);
        await _context.SaveChangesAsync(cancellationToken);
        return batch;
    }

    public async Task DeleteAsync(StockBatch batch, CancellationToken cancellationToken = default)
    {
        _context.StockBatches.Remove(batch);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockBatch>> GetExpiringBeforeAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
        => await _context.StockBatches
            .Where(b => b.ExpiryDate.HasValue && b.ExpiryDate.Value <= cutoffDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}
