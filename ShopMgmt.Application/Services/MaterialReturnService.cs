using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class MaterialReturnService : IMaterialReturnService
{
    private readonly IMaterialReturnRepository _returnRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly IAlertRepository _alertRepository;

    public MaterialReturnService(
        IMaterialReturnRepository returnRepository,
        IMaterialRepository materialRepository,
        IStockBatchRepository stockBatchRepository,
        IAlertRepository alertRepository)
    {
        _returnRepository = returnRepository;
        _materialRepository = materialRepository;
        _stockBatchRepository = stockBatchRepository;
        _alertRepository = alertRepository;
    }

    public async Task<MaterialReturnDto> RecordReturnAsync(
        CreateMaterialReturnDto dto,
        int returnedByUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await _materialRepository.ExistsAsync(dto.MaterialId, cancellationToken))
            throw new NotFoundException($"Material {dto.MaterialId} was not found.");

        if (dto.Quantity <= 0)
            throw new ConflictException("Return quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.Remarks))
            throw new ConflictException("Remarks are required when reporting a defect return.");

        var materialReturn = new MaterialReturn
        {
            MaterialId = dto.MaterialId,
            ShopId = dto.ShopId,
            UsageId = dto.UsageId,
            BatchId = dto.BatchId,
            ReturnedByUserId = returnedByUserId,
            Quantity = dto.Quantity,
            Remarks = dto.Remarks.Trim(),
            ReturnedAt = DateTime.UtcNow
        };

        if (dto.BatchId.HasValue)
        {
            var batch = await _stockBatchRepository.GetByIdAsync(dto.BatchId.Value, cancellationToken);
            if (batch is null || batch.MaterialId != dto.MaterialId)
                throw new NotFoundException($"Batch {dto.BatchId} not found for material {dto.MaterialId}.");

            batch.Status = MaterialStatus.Quarantined;
            batch.QuarantineDate = DateTime.UtcNow;
            batch.QuarantineReason = dto.Remarks.Trim();
            await _stockBatchRepository.UpdateAsync(batch, cancellationToken);
        }

        await _returnRepository.AddAsync(materialReturn, cancellationToken);

        var alertExists = await _alertRepository.ActiveAlertExistsAsync(dto.MaterialId, AlertType.QuarantineReview);
        if (!alertExists)
        {
            await _alertRepository.AddAsync(new Alert
            {
                MaterialId = dto.MaterialId,
                Type = AlertType.QuarantineReview,
                Threshold = 0,
                CurrentQuantity = dto.Quantity,
                TriggeredAt = DateTime.UtcNow,
                CreatedBy = returnedByUserId
            });
        }

        return new MaterialReturnDto
        {
            ReturnId = materialReturn.ReturnId,
            MaterialId = materialReturn.MaterialId,
            ShopId = materialReturn.ShopId,
            UsageId = materialReturn.UsageId,
            BatchId = materialReturn.BatchId,
            Quantity = materialReturn.Quantity,
            Remarks = materialReturn.Remarks,
            ReturnedAt = materialReturn.ReturnedAt
        };
    }
}
