using System.Collections.Generic;
using System.Threading.Tasks;
using ShopMgmt.Application.DTOS;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IMaterialUsageService
{
    Task<MaterialUsageDto> RecordUsageAsync(CreateMaterialUsageDto createUsageDto);
    Task<List<MaterialUsageDto>> GetUsagesByShopAsync(int shopId);
    Task<MaterialUsageDto?> GetUsageByIdAsync(int usageId);
    Task UpdateUsageAsync(int usageId, UpdateMaterialUsageDto updateUsageDto);
    Task DeleteUsageAsync(int usageId);
}
