using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class MaterialReturnRepository : IMaterialReturnRepository
{
    private readonly AppDbContext _context;

    public MaterialReturnRepository(AppDbContext context) => _context = context;

    public async Task<MaterialReturn> AddAsync(MaterialReturn materialReturn, CancellationToken cancellationToken = default)
    {
        _context.MaterialReturns.Add(materialReturn);
        await _context.SaveChangesAsync(cancellationToken);
        return materialReturn;
    }

    public async Task<IReadOnlyList<MaterialReturn>> ListByMaterialAsync(int materialId, CancellationToken cancellationToken = default)
        => await _context.MaterialReturns
            .Where(r => r.MaterialId == materialId)
            .OrderByDescending(r => r.ReturnedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}
