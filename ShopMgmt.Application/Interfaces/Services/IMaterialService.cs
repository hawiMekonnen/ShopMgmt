using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IMaterialService
{
    Task<IReadOnlyList<MaterialListItemDto>> GetAllAsync(int? shopId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialListItemDto>> SearchAsync(string? partNumber, string? aircraft, string? query, int? shopId, CancellationToken cancellationToken = default);
    Task<MaterialDetailDto> GetByIdAsync(int id, int? shopId = null, CancellationToken cancellationToken = default);
    Task<MaterialInventoryDto> GetInventoryAsync(int id, int? shopId = null, CancellationToken cancellationToken = default);
    Task<MaterialDetailDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<MaterialDetailDto> UpdateAsync(int id, UpdateMaterialDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
