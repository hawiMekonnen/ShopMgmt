using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IProcurementService
{
    Task<MaterialRequestDto> MarkReadyAsync(int requestId, string? notes = null, CancellationToken cancellationToken = default);
    Task<MaterialDto> AddMaterialAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProcurementActionDto>> GetActionsAsync(int? shopId = null, CancellationToken cancellationToken = default);
    Task MarkReorderAsync(int materialId, MarkReorderDto dto, CancellationToken cancellationToken = default);
    Task<ProcurementBudgetReportDto> GetBudgetReportAsync(int? shopId = null, CancellationToken cancellationToken = default);
}
