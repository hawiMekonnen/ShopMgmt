using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IProcurementService
{
    Task<IReadOnlyList<ProcurementActionDto>> GetActionsAsync(int? shopId = null, CancellationToken cancellationToken = default);
    Task MarkReorderAsync(int materialId, MarkReorderDto dto, CancellationToken cancellationToken = default);
}
