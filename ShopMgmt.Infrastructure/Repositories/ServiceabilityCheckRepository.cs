using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopMgmt.Infrastructure.Repositories;

public class ServiceabilityCheckRepository : IServiceabilityCheckRepository
{
    private readonly AppDbContext _context;

    public ServiceabilityCheckRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ServiceabilityCheck>> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default) =>
        await _context.ServiceabilityChecks
            .Where(c => c.BatchId == batchId)
            .OrderByDescending(c => c.CheckedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<ServiceabilityCheck?> GetByIdAsync(int checkId, CancellationToken cancellationToken = default) =>
        await _context.ServiceabilityChecks.FindAsync([checkId], cancellationToken);

    public async Task<ServiceabilityCheck> AddAsync(ServiceabilityCheck check, CancellationToken cancellationToken = default)
    {
        _context.ServiceabilityChecks.Add(check);
        await _context.SaveChangesAsync(cancellationToken);
        return check;
    }
}
