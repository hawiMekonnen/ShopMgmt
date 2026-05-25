using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByShopAndRoleAsync(int shopId, UserRole role, CancellationToken cancellationToken = default);
}
