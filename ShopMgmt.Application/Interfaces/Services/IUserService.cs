using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserListItemDto>> GetTechniciansForShopAsync(int shopId, CancellationToken cancellationToken = default);
    Task<UserListItemDto> CreateTechnicianAsync(int shopId, CreateTechnicianDto dto, CancellationToken cancellationToken = default);
    Task<UserListItemDto> CreateShopManagerAsync(CreateShopManagerDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserListItemDto>> GetShopManagersAsync(CancellationToken cancellationToken = default);
    Task<ShopActivityDto> GetShopActivityAsync(int shopId, CancellationToken cancellationToken = default);
}
