using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Services;

public class MaterialUsageService : IMaterialUsageService
{
    private readonly IMaterialUsageRepository _usageRepository;
    private readonly IMaterialRepository _materialRepository;

    public MaterialUsageService(
        IMaterialUsageRepository usageRepository,
        IMaterialRepository materialRepository)
    {
        _usageRepository = usageRepository;
        _materialRepository = materialRepository;
    }

    public async Task<MaterialUsageDto> RecordUsageAsync(CreateMaterialUsageDto createUsageDto)
    {
        await ValidateIssueAsync(
            createUsageDto.MaterialId,
            createUsageDto.ShopId,
            createUsageDto.QuantityUsed);

        var usage = new MaterialUsage
        {
            MaterialId = createUsageDto.MaterialId,
            ShopId = createUsageDto.ShopId,
            QuantityUsed = createUsageDto.QuantityUsed,
            UsedAt = DateTime.UtcNow,
            TailNumber = createUsageDto.TailNumber,
            UserId = createUsageDto.UserId,
            RequestId = createUsageDto.RequestId,
            IssuedByUserId = createUsageDto.IssuedByUserId,
            CollectedByUserId = createUsageDto.CollectedByUserId
        };

        var createdUsage = await _usageRepository.AddAsync(usage);
        var fetchedUsage = await _usageRepository.GetByIdAsync(createdUsage.UsageId);
        return MapToDto(fetchedUsage ?? createdUsage);
    }

    public async Task ValidateIssueAsync(int materialId, int shopId, decimal quantity)
    {
        var inventory = await _materialRepository.GetInventoryAsync(materialId, shopId);
        if (inventory is null)
            throw new NotFoundException($"Material {materialId} was not found.");

        if (quantity <= 0)
            throw new ConflictException("Issue quantity must be greater than zero.");

        if (quantity > inventory.Available)
            throw new ConflictException(
                $"Insufficient serviceable stock. Available: {inventory.Available}, requested: {quantity}.");
    }

    public async Task<List<MaterialUsageDto>> GetUsagesByShopAsync(int shopId)
    {
        var usages = await _usageRepository.GetByShopAsync(shopId);
        return usages.Select(MapToDto).ToList();
    }

    public async Task<MaterialUsageDto?> GetUsageByIdAsync(int usageId)
    {
        var usage = await _usageRepository.GetByIdAsync(usageId);
        if (usage == null) return null;
        return MapToDto(usage);
    }

    public async Task UpdateUsageAsync(int usageId, UpdateMaterialUsageDto updateUsageDto)
    {
        var existingUsage = await _usageRepository.GetByIdAsync(usageId);
        if (existingUsage == null)
            throw new NotFoundException($"Material usage with ID {usageId} not found.");

        await ValidateIssueAsync(
            updateUsageDto.MaterialId,
            updateUsageDto.ShopId,
            updateUsageDto.QuantityUsed);

        existingUsage.MaterialId = updateUsageDto.MaterialId;
        existingUsage.ShopId = updateUsageDto.ShopId;
        existingUsage.QuantityUsed = updateUsageDto.QuantityUsed;
        existingUsage.TailNumber = updateUsageDto.TailNumber;
        existingUsage.UserId = updateUsageDto.UserId;

        await _usageRepository.UpdateAsync(existingUsage);
    }

    public async Task DeleteUsageAsync(int usageId)
    {
        await _usageRepository.DeleteAsync(usageId);
    }

    private static MaterialUsageDto MapToDto(MaterialUsage usage)
    {
        return new MaterialUsageDto
        {
            UsageId = usage.UsageId,
            MaterialId = usage.MaterialId,
            MaterialName = usage.Material?.Name ?? string.Empty,
            ShopId = usage.ShopId,
            ShopName = usage.Shop?.Name ?? string.Empty,
            QuantityUsed = usage.QuantityUsed,
            DateUsed = usage.UsedAt,
            TailNumber = usage.TailNumber,
            UserId = usage.UserId,
            RequestId = usage.RequestId,
            IssuedByUserId = usage.IssuedByUserId,
            CollectedByUserId = usage.CollectedByUserId
        };
    }
}
