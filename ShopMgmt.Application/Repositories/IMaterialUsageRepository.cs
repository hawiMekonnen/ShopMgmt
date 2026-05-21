using ShopMgmt.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopMgmt.Application.Repositories;

public interface IMaterialUsageRepository
{
    Task<MaterialUsage> AddAsync(MaterialUsage usage);
    Task<List<MaterialUsage>> GetByShopAsync(int shopId);
    Task<MaterialUsage?> GetByIdAsync(int usageId);
    Task UpdateAsync(MaterialUsage usage);
    Task DeleteAsync(int usageId);
}
