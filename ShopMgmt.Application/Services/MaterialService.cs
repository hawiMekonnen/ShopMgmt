using AutoMapper;
using FluentValidation;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class MaterialService : IMaterialService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly IMapper _mapper;
    private readonly IAuditRecorder _auditRecorder;
    private readonly IValidator<CreateMaterialDto> _createValidator;
    private readonly IValidator<UpdateMaterialDto> _updateValidator;

    public MaterialService(
        IMaterialRepository materialRepository,
        ICategoryRepository categoryRepository,
        IStockBatchRepository stockBatchRepository,
        IMapper mapper,
        IAuditRecorder auditRecorder,
        IValidator<CreateMaterialDto> createValidator,
        IValidator<UpdateMaterialDto> updateValidator)
    {
        _materialRepository = materialRepository;
        _categoryRepository = categoryRepository;
        _stockBatchRepository = stockBatchRepository;
        _mapper = mapper;
        _auditRecorder = auditRecorder;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<MaterialListItemDto>> GetAllAsync(
        int? shopId = null,
        bool technicianCatalog = false,
        CancellationToken cancellationToken = default)
    {
        var rows = await _materialRepository.GetAllWithInventoryAsync(shopId, cancellationToken);
        return MapCatalogRows(rows, technicianCatalog);
    }

    public async Task<IReadOnlyList<MaterialListItemDto>> SearchAsync(
        string? partNumber,
        string? aircraft,
        string? query,
        int? shopId,
        bool technicianCatalog = false,
        CancellationToken cancellationToken = default)
    {
        var rows = await _materialRepository.SearchAsync(partNumber, aircraft, query, shopId, cancellationToken);
        return MapCatalogRows(rows, technicianCatalog);
    }

    private IReadOnlyList<MaterialListItemDto> MapCatalogRows(
        IReadOnlyList<Models.MaterialListRow> rows,
        bool technicianCatalog)
    {
        var list = new List<MaterialListItemDto>();
        foreach (var row in rows)
        {
            if (technicianCatalog && row.Material.HiddenFromTechnicians)
                continue;

            var dto = _mapper.Map<MaterialListItemDto>(row);
            dto.IsOrderable = row.Available > 0 && !row.Material.HiddenFromTechnicians;
            list.Add(dto);
        }

        return list;
    }

    public async Task<MaterialDetailDto> GetByIdAsync(int id, int? shopId = null, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Material {id} was not found.");

        var inventory = await _materialRepository.GetInventoryAsync(id, shopId, cancellationToken)
            ?? throw new NotFoundException($"Material {id} was not found.");

        var batches = await _stockBatchRepository.GetByMaterialIdAsync(id, cancellationToken);

        var detail = _mapper.Map<MaterialDetailDto>(material);
        detail.OnHand = inventory.OnHand;
        detail.Blocked = inventory.Blocked;
        detail.Reserved = inventory.Reserved;
        detail.Available = inventory.Available;
        detail.StockValue = inventory.StockValue;
        detail.MinStock = material.MinStock;
        detail.DefaultShopId = material.DefaultShopId;
        detail.RecentBatches = _mapper.Map<IReadOnlyList<StockBatchDto>>(batches.OrderByDescending(b => b.ReceivedAt).Take(5).ToList());
        return detail;
    }

    public async Task<MaterialInventoryDto> GetInventoryAsync(int id, int? shopId = null, CancellationToken cancellationToken = default)
    {
        var inventory = await _materialRepository.GetInventoryAsync(id, shopId, cancellationToken)
            ?? throw new NotFoundException($"Material {id} was not found.");

        return _mapper.Map<MaterialInventoryDto>(inventory);
    }

    public async Task<MaterialDetailDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId, cancellationToken))
            throw new NotFoundException($"Category {dto.CategoryId} was not found.");

        if (await _materialRepository.PartNumberExistsAsync(dto.PartNumber.Trim(), cancellationToken: cancellationToken))
            throw new ConflictException($"Part number '{dto.PartNumber}' is already registered.");

        var material = _mapper.Map<Material>(dto);
        material.PartNumber = dto.PartNumber.Trim();
        material.CreatedAt = DateTime.UtcNow;

        var created = await _materialRepository.AddAsync(material, cancellationToken);

        if (dto.InitialQuantity > 0)
        {
            var shopId = dto.InitialShopId ?? dto.DefaultShopId;
            var batch = new StockBatch
            {
                MaterialId = created.MaterialId,
                ShopId = shopId,
                QuantityReceived = dto.InitialQuantity,
                ReceivedAt = DateTime.UtcNow,
                CostTotal = dto.InitialQuantity * dto.UnitPrice,
                Status = MaterialStatus.Serviceable
            };
            await _stockBatchRepository.AddAsync(batch, cancellationToken);
            created.HiddenFromTechnicians = false;
            await _materialRepository.UpdateAsync(created, cancellationToken);
        }
        else
        {
            created.HiddenFromTechnicians = true;
            await _materialRepository.UpdateAsync(created, cancellationToken);
        }

        await _auditRecorder.RecordAsync("Create", nameof(Material), created.MaterialId, $"Created material '{created.Name}'", cancellationToken);

        return await GetByIdAsync(created.MaterialId, dto.DefaultShopId, cancellationToken);
    }

    public async Task SetTechnicianVisibilityAsync(
        int materialId,
        bool hiddenFromTechnicians,
        CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(materialId, cancellationToken)
            ?? throw new NotFoundException($"Material {materialId} was not found.");
        material.HiddenFromTechnicians = hiddenFromTechnicians;
        await _materialRepository.UpdateAsync(material, cancellationToken);
    }

    public async Task SyncTechnicianVisibilityAsync(
        int materialId,
        int? shopId = null,
        CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(materialId, cancellationToken)
            ?? throw new NotFoundException($"Material {materialId} was not found.");
        var inventory = await _materialRepository.GetInventoryAsync(materialId, shopId, cancellationToken);
        if (inventory is null)
            return;

        material.HiddenFromTechnicians = inventory.Available <= 0;
        await _materialRepository.UpdateAsync(material, cancellationToken);
    }

    public async Task<MaterialDetailDto> UpdateAsync(int id, UpdateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var material = await _materialRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Material {id} was not found.");

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId, cancellationToken))
            throw new NotFoundException($"Category {dto.CategoryId} was not found.");

        if (await _materialRepository.PartNumberExistsAsync(dto.PartNumber.Trim(), id, cancellationToken))
            throw new ConflictException($"Part number '{dto.PartNumber}' is already registered.");

        material.PartNumber = dto.PartNumber.Trim();
        material.Name = dto.Name;
        material.Description = dto.Description;
        material.AircraftTypes = dto.AircraftTypes;
        material.CategoryId = dto.CategoryId;
        material.UnitPrice = dto.UnitPrice;
        material.Unit = dto.Unit;
        material.MinStock = dto.MinStock;
        material.DefaultShopId = dto.DefaultShopId;

        await _materialRepository.UpdateAsync(material, cancellationToken);
        await _auditRecorder.RecordAsync("Update", nameof(Material), id, $"Updated material '{material.Name}'", cancellationToken);

        return await GetByIdAsync(id, dto.DefaultShopId, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Material {id} was not found.");

        if (await _materialRepository.HasUsagesAsync(id, cancellationToken))
            throw new ConflictException($"Cannot delete material '{material.Name}' because usage records exist.");

        if (await _materialRepository.HasStockBatchesAsync(id, cancellationToken))
            throw new ConflictException($"Cannot delete material '{material.Name}' because stock batches exist.");

        await _materialRepository.DeleteAsync(material, cancellationToken);
        await _auditRecorder.RecordAsync("Delete", nameof(Material), id, $"Deleted material '{material.Name}'", cancellationToken);
    }
}
