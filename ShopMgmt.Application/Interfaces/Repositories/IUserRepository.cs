using System.Threading;
using System.Threading.Tasks;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    // Additional methods as needed
}
