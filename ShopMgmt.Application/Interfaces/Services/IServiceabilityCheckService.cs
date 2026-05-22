using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services
{
    public interface IServiceabilityCheckService
    {
        Task<ServiceabilityCheckDto> RecordCheckAsync(CreateServiceabilityCheckDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ServiceabilityCheckDto>> GetHistoryByBatchAsync(int batchId, CancellationToken cancellationToken = default);
    }
}
