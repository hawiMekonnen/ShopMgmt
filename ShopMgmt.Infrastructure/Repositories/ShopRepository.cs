using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly AppDbContext _context;
    public ShopRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shop>> GetAllAsync()
    {
        return await _context.Shops.ToListAsync();
    }

    public async Task<Shop?> GetByIdAsync(int id)
    {
        return await _context.Shops.FindAsync(id);
    }

    public async Task<Shop> AddAsync(Shop shop)
    {
        _context.Shops.Add(shop);
        await _context.SaveChangesAsync();
        return shop;
    }

    public async Task UpdateAsync(Shop shop)
    {
        _context.Shops.Update(shop);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var shop = await _context.Shops.FindAsync(id);
        if (shop != null)
        {
            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
        }
    }
}
