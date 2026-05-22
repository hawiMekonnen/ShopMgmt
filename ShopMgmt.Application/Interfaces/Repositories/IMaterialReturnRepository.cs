using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IMaterialReturnRepository
{
    Task<MaterialReturn> AddAsync(MaterialReturn materialReturn, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialReturn>> ListByMaterialAsync(int materialId, CancellationToken cancellationToken = default);
}
