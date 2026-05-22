using ShopMgmt.Application.DTOs;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IMaterialRequestService
{
    Task<MaterialRequestDto> SubmitAsync(CreateMaterialRequestDto dto, int requestedByUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialRequestDto>> ListAsync(int? shopId, RequestStatus? status, int? requestedByUserId, CancellationToken cancellationToken = default);
    Task<MaterialRequestDto> GetByIdAsync(int requestId, CancellationToken cancellationToken = default);
    Task<MaterialRequestDto> ApproveAsync(int requestId, CancellationToken cancellationToken = default);
    Task<MaterialRequestDto> MarkReadyAsync(int requestId, CancellationToken cancellationToken = default);
    Task<MaterialRequestDto> IssueAsync(int requestId, IssueMaterialRequestDto dto, int issuedByUserId, CancellationToken cancellationToken = default);
    Task<MaterialRequestDto> CancelAsync(int requestId, CancelMaterialRequestDto? dto, CancellationToken cancellationToken = default);
}
