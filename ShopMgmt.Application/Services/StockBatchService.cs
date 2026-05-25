using AutoMapper;
using FluentValidation;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Services;

public class StockBatchService : IStockBatchService
{
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;
    private readonly IAuditRecorder _auditRecorder;
    private readonly IValidator<CreateStockBatchDto> _createValidator;
    private readonly IAlertService _alertService;
    private readonly IMaterialService _materialService;

    public StockBatchService(
        IStockBatchRepository stockBatchRepository,
        IMaterialRepository materialRepository,
        IMapper mapper,
        IAuditRecorder auditRecorder,
        IValidator<CreateStockBatchDto> createValidator,
        IAlertService alertService,
        IMaterialService materialService)
    {
        _stockBatchRepository = stockBatchRepository;
        _materialRepository = materialRepository;
        _mapper = mapper;
        _auditRecorder = auditRecorder;
        _createValidator = createValidator;
        _alertService = alertService;
        _materialService = materialService;
    }

    public async Task<IReadOnlyList<StockBatchDto>> GetByMaterialIdAsync(int materialId, CancellationToken cancellationToken = default)
    {
        if (!await _materialRepository.ExistsAsync(materialId, cancellationToken))
            throw new NotFoundException($"Material {materialId} was not found.");

        var batches = await _stockBatchRepository.GetByMaterialIdAsync(materialId, cancellationToken);
        return _mapper.Map<IReadOnlyList<StockBatchDto>>(batches);
    }

    public async Task<StockBatchDto> ReceiveAsync(int materialId, CreateStockBatchDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        if (!await _materialRepository.ExistsAsync(materialId, cancellationToken))
            throw new NotFoundException($"Material {materialId} was not found.");

        var batch = _mapper.Map<StockBatch>(dto);
        batch.MaterialId = materialId;
        batch.Status = Domain.Enums.MaterialStatus.Pending;

        var created = await _stockBatchRepository.AddAsync(batch, cancellationToken);
        await _auditRecorder.RecordAsync("Receive", nameof(StockBatch), created.BatchId,
            $"Received {created.QuantityReceived} units for material {materialId}", cancellationToken);

        await _alertService.CheckAndCreateLowStockAlertsAsync();
        await _alertService.CheckAndCreateExpiryAlertsAsync();
        await _materialService.SyncTechnicianVisibilityAsync(materialId, dto.ShopId, cancellationToken);

        return _mapper.Map<StockBatchDto>(created);
    }

    public async Task DeleteAsync(int materialId, int batchId, CancellationToken cancellationToken = default)
    {
        if (!await _materialRepository.ExistsAsync(materialId, cancellationToken))
            throw new NotFoundException($"Material {materialId} was not found.");

        var batch = await _stockBatchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null || batch.MaterialId != materialId)
            throw new NotFoundException($"Stock batch {batchId} was not found for material {materialId}.");

        await _stockBatchRepository.DeleteAsync(batch, cancellationToken);
        await _auditRecorder.RecordAsync("Delete", nameof(StockBatch), batchId,
            $"Deleted stock batch for material {materialId}", cancellationToken);

        await _alertService.CheckAndCreateLowStockAlertsAsync();
        await _materialService.SyncTechnicianVisibilityAsync(materialId, batch.ShopId, cancellationToken);
    }
}
