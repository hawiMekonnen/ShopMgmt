using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<IReadOnlyList<User>> GetByShopAndRoleAsync(
        int shopId,
        UserRole role,
        CancellationToken cancellationToken = default)
        => await _context.Users
            .Where(u => u.ShopId == shopId && u.Role == role)
            .OrderBy(u => u.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}
