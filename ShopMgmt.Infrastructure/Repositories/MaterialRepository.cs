using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Models;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<MaterialListRow>> GetAllWithInventoryAsync(CancellationToken cancellationToken = default)
    {
        var query =
            from m in _context.Materials
            join c in _context.Categories on m.CategoryId equals c.CategoryId
            let received = _context.StockBatches
                .Where(b => b.MaterialId == m.MaterialId)
                .Select(b => (decimal?)b.QuantityReceived)
                .Sum() ?? 0m
            let used = _context.MaterialUsages
                .Where(u => u.MaterialId == m.MaterialId)
                .Select(u => (decimal?)u.QuantityUsed)
                .Sum() ?? 0m
            let onHand = received - used
            orderby m.Name
            select new MaterialListRow
            {
                Material = m,
                CategoryName = c.Name,
                OnHand = onHand,
                StockValue = onHand * m.UnitPrice
            };

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Material?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Materials
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.MaterialId == id, cancellationToken);

    public async Task<MaterialInventorySnapshot?> GetInventoryAsync(int materialId, CancellationToken cancellationToken = default)
    {
        var snapshot = await (
            from m in _context.Materials
            where m.MaterialId == materialId
            let received = _context.StockBatches
                .Where(b => b.MaterialId == m.MaterialId)
                .Select(b => (decimal?)b.QuantityReceived)
                .Sum() ?? 0m
            let used = _context.MaterialUsages
                .Where(u => u.MaterialId == m.MaterialId)
                .Select(u => (decimal?)u.QuantityUsed)
                .Sum() ?? 0m
            let onHand = received - used
            select new MaterialInventorySnapshot
            {
                MaterialId = m.MaterialId,
                OnHand = onHand,
                StockValue = onHand * m.UnitPrice
            }).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        return snapshot;
    }

    public async Task<bool> HasUsagesAsync(int materialId, CancellationToken cancellationToken = default)
        => await _context.MaterialUsages.AnyAsync(u => u.MaterialId == materialId, cancellationToken);

    public async Task<bool> HasStockBatchesAsync(int materialId, CancellationToken cancellationToken = default)
        => await _context.StockBatches.AnyAsync(b => b.MaterialId == materialId, cancellationToken);

    public async Task<Material> AddAsync(Material material, CancellationToken cancellationToken = default)
    {
        _context.Materials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(material.MaterialId, cancellationToken) ?? material;
    }

    public async Task UpdateAsync(Material material, CancellationToken cancellationToken = default)
    {
        _context.Materials.Update(material);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Material material, CancellationToken cancellationToken = default)
    {
        _context.Materials.Remove(material);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Materials.AnyAsync(m => m.MaterialId == id, cancellationToken);
}
