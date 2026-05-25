using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class MaterialUsageRepository : IMaterialUsageRepository
{
    private readonly AppDbContext _context;

    public MaterialUsageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MaterialUsage> AddAsync(MaterialUsage usage)
    {
        _context.MaterialUsages.Add(usage);
        await _context.SaveChangesAsync();
        return usage;
    }

    public async Task<List<MaterialUsage>> GetByShopAsync(int shopId)
    {
        return await _context.MaterialUsages
            .Include(mu => mu.Material)
            .Include(mu => mu.User)
            .Where(mu => mu.ShopId == shopId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<MaterialUsage>> GetRecentByShopAsync(
        int shopId,
        int take,
        CancellationToken cancellationToken = default)
        => await _context.MaterialUsages
            .Include(mu => mu.Material)
            .Include(mu => mu.Shop)
            .Include(mu => mu.User)
            .Where(mu => mu.ShopId == shopId)
            .OrderByDescending(mu => mu.UsedAt)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<MaterialUsage?> GetByIdAsync(int usageId)
    {
        return await _context.MaterialUsages
            .Include(mu => mu.Material)
            .Include(mu => mu.Shop)
            .Include(mu => mu.User)
            .FirstOrDefaultAsync(mu => mu.UsageId == usageId);
    }

    public async Task UpdateAsync(MaterialUsage usage)
    {
        _context.MaterialUsages.Update(usage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int usageId)
    {
        var usage = await _context.MaterialUsages.FindAsync(usageId);
        if (usage != null)
        {
            _context.MaterialUsages.Remove(usage);
            await _context.SaveChangesAsync();
        }
    }
}
