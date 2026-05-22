using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories
{
    public interface IServiceabilityCheckRepository
    {
        Task<IReadOnlyList<ServiceabilityCheck>> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default);
        Task<ServiceabilityCheck?> GetByIdAsync(int checkId, CancellationToken cancellationToken = default);
        Task<ServiceabilityCheck> AddAsync(ServiceabilityCheck check, CancellationToken cancellationToken = default);
    }
}
