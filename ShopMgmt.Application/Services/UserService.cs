using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMaterialRequestRepository _requestRepository;
    private readonly IMaterialUsageRepository _usageRepository;

    public UserService(
        IUserRepository userRepository,
        IMaterialRequestRepository requestRepository,
        IMaterialUsageRepository usageRepository)
    {
        _userRepository = userRepository;
        _requestRepository = requestRepository;
        _usageRepository = usageRepository;
    }

    public async Task<IReadOnlyList<UserListItemDto>> GetTechniciansForShopAsync(
        int shopId,
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByShopAndRoleAsync(shopId, UserRole.Technician, cancellationToken);
        return users.Select(MapUser).ToList();
    }

    public async Task<UserListItemDto> CreateTechnicianAsync(
        int shopId,
        CreateTechnicianDto dto,
        CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ConflictException("Name, email, and password are required.");

        if (dto.Password.Length < 6)
            throw new ConflictException("Password must be at least 6 characters.");

        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
            throw new ConflictException($"Email '{email}' is already registered.");

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Technician,
            ShopId = shopId
        };

        var created = await _userRepository.AddAsync(user, cancellationToken);
        return MapUser(created);
    }

    public async Task<ShopActivityDto> GetShopActivityAsync(int shopId, CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.ListAsync(shopId, null, null, cancellationToken);
        var usages = await _usageRepository.GetRecentByShopAsync(shopId, 50, cancellationToken);
        var technicians = await _userRepository.GetByShopAndRoleAsync(shopId, UserRole.Technician, cancellationToken);

        return new ShopActivityDto
        {
            Requests = requests.Select(MapRequest).ToList(),
            RecentUsages = usages.Select(u => new MaterialUsageActivityDto
            {
                UsageId = u.UsageId,
                MaterialId = u.MaterialId,
                MaterialName = u.Material?.Name ?? string.Empty,
                PartNumber = u.Material?.PartNumber ?? string.Empty,
                ShopId = u.ShopId,
                ShopName = u.Shop?.Name ?? string.Empty,
                UserId = u.UserId,
                UserName = u.User?.Name ?? string.Empty,
                QuantityUsed = u.QuantityUsed,
                UsedAt = u.UsedAt,
                FlightNumber = u.TailNumber
            }).ToList(),
            Technicians = technicians.Select(MapUser).ToList()
        };
    }

    private static UserListItemDto MapUser(User user) => new()
    {
        UserId = user.UserId,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.ToString(),
        ShopId = user.ShopId
    };

    private static MaterialRequestDto MapRequest(MaterialRequest request) => new()
    {
        RequestId = request.RequestId,
        MaterialId = request.MaterialId,
        MaterialName = request.Material?.Name ?? string.Empty,
        PartNumber = request.Material?.PartNumber ?? string.Empty,
        ShopId = request.ShopId,
        ShopName = request.Shop?.Name ?? string.Empty,
        RequestedByUserId = request.RequestedByUserId,
        RequestedByName = request.RequestedBy?.Name ?? string.Empty,
        Quantity = request.Quantity,
        AircraftOrWorkOrder = request.AircraftOrWorkOrder,
        Status = request.Status,
        Notes = request.Notes,
        CreatedAt = request.CreatedAt,
        ApprovedAt = request.ApprovedAt,
        ReadyAt = request.ReadyAt,
        IssuedAt = request.IssuedAt
    };
}
