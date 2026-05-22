using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Inventory;
using ShopMgmt.Application.Models;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<MaterialListRow>> GetAllWithInventoryAsync(
        int? shopId = null,
        CancellationToken cancellationToken = default)
    {
        var materials = await _context.Materials
            .Include(m => m.Category)
            .OrderBy(m => m.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var rows = new List<MaterialListRow>();
        foreach (var material in materials)
        {
            var metrics = await BuildMetricsAsync(material.MaterialId, material.UnitPrice, shopId, cancellationToken);
            rows.Add(new MaterialListRow
            {
                Material = material,
                CategoryName = material.Category.Name,
                OnHand = metrics.OnHand,
                Blocked = metrics.Blocked,
                Reserved = metrics.Reserved,
                Available = metrics.Available,
                StockValue = metrics.StockValue
            });
        }

        return rows;
    }

    public async Task<IReadOnlyList<MaterialListRow>> SearchAsync(
        string? partNumber,
        string? aircraft,
        string? query,
        int? shopId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Materials.Include(m => m.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(partNumber))
            q = q.Where(m => m.PartNumber.Contains(partNumber));

        if (!string.IsNullOrWhiteSpace(aircraft))
            q = q.Where(m => m.AircraftTypes != null && m.AircraftTypes.Contains(aircraft));

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            q = q.Where(m =>
                m.Name.Contains(term) ||
                m.PartNumber.Contains(term) ||
                (m.Description != null && m.Description.Contains(term)));
        }

        var materials = await q.OrderBy(m => m.PartNumber).AsNoTracking().ToListAsync(cancellationToken);
        var rows = new List<MaterialListRow>();
        foreach (var material in materials)
        {
            var metrics = await BuildMetricsAsync(material.MaterialId, material.UnitPrice, shopId, cancellationToken);
            rows.Add(new MaterialListRow
            {
                Material = material,
                CategoryName = material.Category.Name,
                OnHand = metrics.OnHand,
                Blocked = metrics.Blocked,
                Reserved = metrics.Reserved,
                Available = metrics.Available,
                StockValue = metrics.StockValue
            });
        }

        return rows;
    }

    public async Task<Material?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Materials
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.MaterialId == id, cancellationToken);

    public async Task<MaterialInventorySnapshot?> GetInventoryAsync(
        int materialId,
        int? shopId = null,
        CancellationToken cancellationToken = default)
    {
        var material = await _context.Materials
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MaterialId == materialId, cancellationToken);

        if (material is null)
            return null;

        var metrics = await BuildMetricsAsync(materialId, material.UnitPrice, shopId, cancellationToken);
        return new MaterialInventorySnapshot
        {
            MaterialId = materialId,
            OnHand = metrics.OnHand,
            Blocked = metrics.Blocked,
            Reserved = metrics.Reserved,
            Available = metrics.Available,
            StockValue = metrics.StockValue
        };
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

    public async Task<bool> PartNumberExistsAsync(string partNumber, int? excludeMaterialId = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Materials.Where(m => m.PartNumber == partNumber);
        if (excludeMaterialId.HasValue)
            q = q.Where(m => m.MaterialId != excludeMaterialId.Value);
        return await q.AnyAsync(cancellationToken);
    }

    private async Task<MaterialInventorySnapshot> BuildMetricsAsync(
        int materialId,
        decimal unitPrice,
        int? shopId,
        CancellationToken cancellationToken)
    {
        var batches = _context.StockBatches.Where(b => b.MaterialId == materialId);
        if (shopId.HasValue)
            batches = batches.Where(b => b.ShopId == null || b.ShopId == shopId.Value);

        var batchList = await batches.AsNoTracking().ToListAsync(cancellationToken);

        var serviceableReceived = batchList.Where(b => b.Status == MaterialStatus.Serviceable).Sum(b => b.QuantityReceived);
        var pendingReceived = batchList.Where(b => b.Status == MaterialStatus.Pending).Sum(b => b.QuantityReceived);
        var quarantinedReceived = batchList.Where(b => b.Status == MaterialStatus.Quarantined).Sum(b => b.QuantityReceived);
        var condemnedReceived = batchList.Where(b => b.Status == MaterialStatus.Condemned).Sum(b => b.QuantityReceived);

        var usages = _context.MaterialUsages.Where(u => u.MaterialId == materialId);
        if (shopId.HasValue)
            usages = usages.Where(u => u.ShopId == shopId.Value);

        var totalUsed = await usages.SumAsync(u => (decimal?)u.QuantityUsed, cancellationToken) ?? 0m;

        var reserved = await GetReservedQuantityAsync(materialId, shopId, cancellationToken);

        var (onHand, blocked, available, stockValue) = InventoryCalculator.Compute(
            serviceableReceived,
            pendingReceived,
            quarantinedReceived,
            condemnedReceived,
            totalUsed,
            reserved,
            unitPrice);

        return new MaterialInventorySnapshot
        {
            MaterialId = materialId,
            OnHand = onHand,
            Blocked = blocked,
            Reserved = reserved,
            Available = available,
            StockValue = stockValue
        };
    }

    private async Task<decimal> GetReservedQuantityAsync(
        int materialId,
        int? shopId,
        CancellationToken cancellationToken)
    {
        var q = _context.MaterialRequests.Where(r =>
            r.MaterialId == materialId &&
            (r.Status == RequestStatus.Approved || r.Status == RequestStatus.ReadyForPickup));

        if (shopId.HasValue)
            q = q.Where(r => r.ShopId == shopId.Value);

        return await q.SumAsync(r => (decimal?)r.Quantity, cancellationToken) ?? 0m;
    }
}
