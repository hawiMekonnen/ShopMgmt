using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class MaterialRequestRepository : IMaterialRequestRepository
{
    private readonly AppDbContext _context;

    public MaterialRequestRepository(AppDbContext context) => _context = context;

    public async Task<MaterialRequest?> GetByIdAsync(int requestId, CancellationToken cancellationToken = default)
        => await _context.MaterialRequests
            .Include(r => r.Material)
            .Include(r => r.Shop)
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.RequestId == requestId, cancellationToken);

    public async Task<IReadOnlyList<MaterialRequest>> ListAsync(
        int? shopId,
        RequestStatus? status,
        int? requestedByUserId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.MaterialRequests
            .Include(r => r.Material)
            .Include(r => r.Shop)
            .Include(r => r.RequestedBy)
            .AsQueryable();

        if (shopId.HasValue)
            q = q.Where(r => r.ShopId == shopId.Value);
        if (status.HasValue)
            q = q.Where(r => r.Status == status.Value);
        if (requestedByUserId.HasValue)
            q = q.Where(r => r.RequestedByUserId == requestedByUserId.Value);

        return await q.OrderByDescending(r => r.CreatedAt).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<MaterialRequest> AddAsync(MaterialRequest request, CancellationToken cancellationToken = default)
    {
        _context.MaterialRequests.Add(request);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(request.RequestId, cancellationToken))!;
    }

    public async Task UpdateAsync(MaterialRequest request, CancellationToken cancellationToken = default)
    {
        _context.MaterialRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
