using ShopMgmt.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IShopRepository
{
    Task<List<Shop>> GetAllAsync();
    Task<Shop?> GetByIdAsync(int id);
    Task<Shop> AddAsync(Shop shop);
    Task UpdateAsync(Shop shop);
    Task DeleteAsync(int id);
}
