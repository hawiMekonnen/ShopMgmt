using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using AutoMapper;

namespace ShopMgmt.Application.Services;

public class ProcurementService : IProcurementService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly IMaterialRequestRepository _requestRepository;
    private readonly IMaterialService _materialService;
    private readonly IAlertService _alertService;
    private readonly IMapper _mapper;

    public ProcurementService(
        IMaterialRepository materialRepository,
        IStockBatchRepository stockBatchRepository,
        IMaterialRequestRepository requestRepository,
        IMaterialService materialService,
        IAlertService alertService,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _stockBatchRepository = stockBatchRepository;
        _requestRepository = requestRepository;
        _materialService = materialService;
        _alertService = alertService;
        _mapper = mapper;
    }

    public async Task<MaterialDto> AddMaterialAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        var createdDetail = await _materialService.CreateAsync(dto, cancellationToken);
        return _mapper.Map<MaterialDto>(createdDetail);
    }

    public async Task MarkReorderAsync(int materialId, MarkReorderDto dto, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(materialId, cancellationToken)
            ?? throw new NotFoundException($"Material {materialId} was not found.");

        material.ReorderPlaced = true;
        material.ReorderNote = dto.ReorderNote?.Trim();
        await _materialRepository.UpdateAsync(material, cancellationToken);
    }

    public async Task<MaterialRequestDto> MarkReadyAsync(int requestId, string? notes = null, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException($"Request {requestId} was not found.");

        if (request.Status != RequestStatus.Approved)
            throw new ConflictException($"Request must be approved before marking ready. Current: {request.Status}.");

        request.Status = RequestStatus.ReadyForPickup;
        request.ReadyAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            request.Notes = notes;
        }
        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _alertService.CreatePickupReadyAlertAsync(
            request.MaterialId,
            request.RequestId,
            request.Quantity,
            request.RequestedByUserId,
            cancellationToken);

        return MapToDto((await _requestRepository.GetByIdAsync(requestId, cancellationToken))!);
    }

    public async Task<IReadOnlyList<ProcurementActionDto>> GetActionsAsync(
        int? shopId = null,
        CancellationToken cancellationToken = default)
    {
        var rows = await _materialRepository.GetAllWithInventoryAsync(shopId, cancellationToken);

        var actions = new List<ProcurementActionDto>();

        foreach (var row in rows)
        {
            var material = row.Material;

            // Low stock action — available stock at or below minimum
            if (row.Available <= material.MinStock)
            {
                actions.Add(new ProcurementActionDto
                {
                    MaterialId = material.MaterialId,
                    PartNumber = material.PartNumber,
                    MaterialName = material.Name,
                    ActionType = material.ReorderPlaced ? "ReorderInProgress" : "ReorderRequired",
                    Summary = material.ReorderPlaced
                        ? $"Reorder already placed. Available: {row.Available} {material.Unit}, Min: {material.MinStock} {material.Unit}. Note: {material.ReorderNote}"
                        : $"Stock low. Available: {row.Available} {material.Unit}, Min: {material.MinStock} {material.Unit}.",
                    Quantity = row.Available,
                    RelatedId = null,
                    ReorderPlaced = material.ReorderPlaced,
                    ReorderNote = material.ReorderNote
                });
            }

            // Blocked stock action — quarantined or condemned batches sitting unresolved
            if (row.Blocked > 0)
            {
                actions.Add(new ProcurementActionDto
                {
                    MaterialId = material.MaterialId,
                    PartNumber = material.PartNumber,
                    MaterialName = material.Name,
                    ActionType = "BlockedStockAlert",
                    Summary = $"{row.Blocked} {material.Unit} blocked (quarantined/condemned) and requires review.",
                    Quantity = row.Blocked,
                    RelatedId = null,
                    ReorderPlaced = material.ReorderPlaced,
                    ReorderNote = material.ReorderNote
                });
            }
        }

        return actions.AsReadOnly();
    }

    private static MaterialRequestDto MapToDto(MaterialRequest request) => new()
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