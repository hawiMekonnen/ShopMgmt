using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IMaterialReturnService
{
    Task<MaterialReturnDto> RecordReturnAsync(CreateMaterialReturnDto dto, int returnedByUserId, CancellationToken cancellationToken = default);
}
