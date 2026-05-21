using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Services;

public class MaterialUsageService : IMaterialUsageService
{
    private readonly IMaterialUsageRepository _usageRepository;

    public MaterialUsageService(IMaterialUsageRepository usageRepository)
    {
        _usageRepository = usageRepository;
    }

    public async Task<MaterialUsageDto> RecordUsageAsync(CreateMaterialUsageDto createUsageDto)
    {
        var usage = new MaterialUsage
        {
            MaterialId = createUsageDto.MaterialId,
            ShopId = createUsageDto.ShopId,
            QuantityUsed = createUsageDto.QuantityUsed,
            UsedAt = DateTime.UtcNow,
            FlightNumber = createUsageDto.FlightNumber,
            UserId = createUsageDto.UserId
        };

        var createdUsage = await _usageRepository.AddAsync(usage);
        
        // Let's refetch to get nav properties for the DTO
        var fetchedUsage = await _usageRepository.GetByIdAsync(createdUsage.UsageId);

        return MapToDto(fetchedUsage ?? createdUsage);
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
        {
            throw new Exception($"Material usage with ID {usageId} not found.");
        }

        existingUsage.MaterialId = updateUsageDto.MaterialId;
        existingUsage.ShopId = updateUsageDto.ShopId;
        existingUsage.QuantityUsed = updateUsageDto.QuantityUsed;
        existingUsage.FlightNumber = updateUsageDto.FlightNumber;
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
            FlightNumber = usage.FlightNumber,
            UserId = usage.UserId
        };
    }
}
