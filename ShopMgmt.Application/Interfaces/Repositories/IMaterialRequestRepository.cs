using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IMaterialRequestRepository
{
    Task<MaterialRequest?> GetByIdAsync(int requestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialRequest>> ListAsync(int? shopId, RequestStatus? status, int? requestedByUserId, CancellationToken cancellationToken = default);
    Task<MaterialRequest> AddAsync(MaterialRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(MaterialRequest request, CancellationToken cancellationToken = default);
}
