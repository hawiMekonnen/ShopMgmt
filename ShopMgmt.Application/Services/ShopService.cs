using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;

    public ShopService(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }

    public async Task<List<ShopDto>> GetAllShopsAsync()
    {
        var shops = await _shopRepository.GetAllAsync();
        return shops.Select(s => new ShopDto
        {
            ShopId = s.ShopId,
            Name = s.Name,
            Location = s.Location
        }).ToList();
    }

    public async Task<ShopDto?> GetShopByIdAsync(int id)
    {
        var shop = await _shopRepository.GetByIdAsync(id);
        if (shop == null) return null;

        return new ShopDto
        {
            ShopId = shop.ShopId,
            Name = shop.Name,
            Location = shop.Location
        };
    }

    public async Task<ShopDto> CreateShopAsync(CreateShopDto createShopDto)
    {
        var shop = new Shop
        {
            Name = createShopDto.Name,
            Location = createShopDto.Location
        };

        var createdShop = await _shopRepository.AddAsync(shop);

        return new ShopDto
        {
            ShopId = createdShop.ShopId,
            Name = createdShop.Name,
            Location = createdShop.Location
        };
    }

    public async Task UpdateShopAsync(int id, UpdateShopDto updateShopDto)
    {
        var existingShop = await _shopRepository.GetByIdAsync(id);
        if (existingShop == null)
        {
            throw new Exception($"Shop with ID {id} not found.");
        }

        existingShop.Name = updateShopDto.Name;
        existingShop.Location = updateShopDto.Location;

        await _shopRepository.UpdateAsync(existingShop);
    }

    public async Task DeleteShopAsync(int id)
    {
        await _shopRepository.DeleteAsync(id);
    }
}
