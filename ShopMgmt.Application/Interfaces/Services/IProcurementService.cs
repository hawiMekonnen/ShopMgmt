using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IProcurementService
{
    Task<IReadOnlyList<ProcurementActionDto>> GetActionsAsync(CancellationToken cancellationToken = default);
    Task MarkReorderAsync(int materialId, MarkReorderDto dto, CancellationToken cancellationToken = default);
}
