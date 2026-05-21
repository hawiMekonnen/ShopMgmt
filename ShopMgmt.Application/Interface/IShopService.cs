using System.Collections.Generic;
using System.Threading.Tasks;
using ShopMgmt.Application.DTOS;

namespace ShopMgmt.Application.Interface;

public interface IShopService
{
    Task<List<ShopDto>> GetAllShopsAsync();
    Task<ShopDto?> GetShopByIdAsync(int id);
    Task<ShopDto> CreateShopAsync(CreateShopDto createShopDto);
    Task UpdateShopAsync(int id, UpdateShopDto updateShopDto);
    Task DeleteShopAsync(int id);
}
