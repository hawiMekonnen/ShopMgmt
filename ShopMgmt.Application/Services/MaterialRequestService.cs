using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class MaterialRequestService : IMaterialRequestService
{
    private readonly IMaterialRequestRepository _requestRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IMaterialUsageService _usageService;
    private readonly IAlertRepository _alertRepository;

    public MaterialRequestService(
        IMaterialRequestRepository requestRepository,
        IMaterialRepository materialRepository,
        IMaterialUsageService usageService,
        IAlertRepository alertRepository)
    {
        _requestRepository = requestRepository;
        _materialRepository = materialRepository;
        _usageService = usageService;
        _alertRepository = alertRepository;
    }

    public async Task<MaterialRequestDto> SubmitAsync(
        CreateMaterialRequestDto dto,
        int requestedByUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await _materialRepository.ExistsAsync(dto.MaterialId, cancellationToken))
            throw new NotFoundException($"Material {dto.MaterialId} was not found.");

        var inventory = await _materialRepository.GetInventoryAsync(dto.MaterialId, dto.ShopId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Material {dto.MaterialId} was not found.");

        if (dto.Quantity <= 0)
            throw new ConflictException("Request quantity must be greater than zero.");

        if (dto.Quantity > inventory.Available)
            throw new ConflictException(
                $"Insufficient available stock. Available: {inventory.Available}, requested: {dto.Quantity}.");

        var workOrder = dto.AircraftOrWorkOrder?.Trim();
        if (string.IsNullOrWhiteSpace(workOrder))
            throw new ConflictException("Work order is required.");

        var request = new MaterialRequest
        {
            MaterialId = dto.MaterialId,
            ShopId = dto.ShopId,
            RequestedByUserId = requestedByUserId,
            Quantity = dto.Quantity,
            AircraftOrWorkOrder = workOrder,
            Notes = dto.Notes,
            Status = RequestStatus.Submitted,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _requestRepository.AddAsync(request, cancellationToken);
        return MapToDto(created);
    }

    public async Task<IReadOnlyList<MaterialRequestDto>> ListAsync(
        int? shopId,
        RequestStatus? status,
        int? requestedByUserId,
        CancellationToken cancellationToken = default)
    {
        var list = await _requestRepository.ListAsync(shopId, status, requestedByUserId, cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<MaterialRequestDto> GetByIdAsync(int requestId, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException($"Request {requestId} was not found.");
        return MapToDto(request);
    }

    public async Task<MaterialRequestDto> ReleaseForIssueAsync(int requestId, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException($"Request {requestId} was not found.");

        if (request.Status is RequestStatus.ReadyForPickup or RequestStatus.Issued or RequestStatus.Cancelled)
            throw new ConflictException($"Request cannot be released in status {request.Status}.");

        if (request.Status == RequestStatus.Submitted)
        {
            var inventory = await _materialRepository.GetInventoryAsync(request.MaterialId, request.ShopId, cancellationToken);
            if (inventory is null || request.Quantity > inventory.Available)
                throw new ConflictException("Cannot release: insufficient available stock.");
        }
        else if (request.Status != RequestStatus.Approved)
        {
            throw new ConflictException($"Request must be submitted to release. Current: {request.Status}.");
        }

        var now = DateTime.UtcNow;
        request.Status = RequestStatus.ReadyForPickup;
        request.ApprovedAt ??= now;
        request.ReadyAt = now;
        await _requestRepository.UpdateAsync(request, cancellationToken);
        await EnsurePickupReadyAlertAsync(request, cancellationToken);

        return MapToDto((await _requestRepository.GetByIdAsync(requestId, cancellationToken))!);
    }

    private async Task EnsurePickupReadyAlertAsync(MaterialRequest request, CancellationToken cancellationToken)
    {
        var exists = await _alertRepository.ActivePickupAlertExistsForRequestAsync(request.RequestId);
        if (exists) return;

        await _alertRepository.AddAsync(new Alert
        {
            MaterialId = request.MaterialId,
            Type = AlertType.PickupReady,
            Threshold = 0,
            CurrentQuantity = request.Quantity,
            TriggeredAt = DateTime.UtcNow,
            CreatedBy = request.RequestedByUserId,
            RequestId = request.RequestId
        });
    }

    public async Task<MaterialRequestDto> IssueAsync(
        int requestId,
        IssueMaterialRequestDto dto,
        int issuedByUserId,
        CancellationToken cancellationToken = default)
    {
        var request = await GetRequestForTransitionAsync(requestId, RequestStatus.ReadyForPickup, cancellationToken);

        await _usageService.RecordUsageAsync(new DTOS.CreateMaterialUsageDto
        {
            MaterialId = request.MaterialId,
            ShopId = request.ShopId,
            QuantityUsed = request.Quantity,
            FlightNumber = dto.FlightNumber ?? request.AircraftOrWorkOrder,
            UserId = request.RequestedByUserId,
            RequestId = request.RequestId,
            IssuedByUserId = issuedByUserId,
            CollectedByUserId = dto.CollectedByUserId
        });

        request.Status = RequestStatus.Issued;
        request.IssuedAt = DateTime.UtcNow;
        await _requestRepository.UpdateAsync(request, cancellationToken);

        return MapToDto((await _requestRepository.GetByIdAsync(requestId, cancellationToken))!);
    }

    public async Task<MaterialRequestDto> CancelAsync(
        int requestId,
        CancelMaterialRequestDto? dto,
        CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException($"Request {requestId} was not found.");

        if (request.Status is RequestStatus.Issued or RequestStatus.Cancelled)
            throw new ConflictException($"Request cannot be cancelled in status {request.Status}.");

        request.Status = RequestStatus.Cancelled;
        if (dto?.Notes is not null)
            request.Notes = dto.Notes;
        await _requestRepository.UpdateAsync(request, cancellationToken);
        return MapToDto((await _requestRepository.GetByIdAsync(requestId, cancellationToken))!);
    }

    private async Task<MaterialRequest> GetRequestForTransitionAsync(
        int requestId,
        RequestStatus expectedStatus,
        CancellationToken cancellationToken)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException($"Request {requestId} was not found.");

        if (request.Status != expectedStatus)
            throw new ConflictException($"Request must be in {expectedStatus} status. Current: {request.Status}.");

        return request;
    }

    private static MaterialRequestDto MapToDto(MaterialRequest request) =>
        new()
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
