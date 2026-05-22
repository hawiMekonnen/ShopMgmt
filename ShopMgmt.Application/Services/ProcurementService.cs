using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class ProcurementService : IProcurementService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly IMaterialRequestRepository _requestRepository;

    public ProcurementService(
        IMaterialRepository materialRepository,
        IStockBatchRepository stockBatchRepository,
        IMaterialRequestRepository requestRepository)
    {
        _materialRepository = materialRepository;
        _stockBatchRepository = stockBatchRepository;
        _requestRepository = requestRepository;
    }

    public async Task<IReadOnlyList<ProcurementActionDto>> GetActionsAsync(
        int? shopId = null,
        CancellationToken cancellationToken = default)
    {
        var actions = new List<ProcurementActionDto>();

        var rows = await _materialRepository.GetAllWithInventoryAsync(shopId, cancellationToken);
        foreach (var row in rows.Where(r => r.Available < r.Material.MinStock && r.Material.MinStock > 0))
        {
            actions.Add(new ProcurementActionDto
            {
                MaterialId = row.Material.MaterialId,
                PartNumber = row.Material.PartNumber,
                MaterialName = row.Material.Name,
                ActionType = "LowStock",
                Summary = $"Available {row.Available} below min {row.Material.MinStock}",
                Quantity = row.Available,
                ReorderPlaced = row.Material.ReorderPlaced,
                ReorderNote = row.Material.ReorderNote
            });
        }

        var quarantined = await _stockBatchRepository.GetQuarantinedAsync(cancellationToken);
        if (shopId.HasValue)
            quarantined = quarantined.Where(b => b.ShopId == null || b.ShopId == shopId.Value).ToList();

        foreach (var batch in quarantined)
        {
            actions.Add(new ProcurementActionDto
            {
                MaterialId = batch.MaterialId,
                PartNumber = batch.Material.PartNumber,
                MaterialName = batch.Material.Name,
                ActionType = "Quarantine",
                Summary = batch.QuarantineReason ?? "Batch quarantined",
                Quantity = batch.QuantityReceived,
                RelatedId = batch.BatchId,
                ReorderPlaced = batch.Material.ReorderPlaced,
                ReorderNote = batch.Material.ReorderNote
            });
        }

        var openRequests = await _requestRepository.ListAsync(shopId, null, null, cancellationToken);
        foreach (var req in openRequests.Where(r =>
                     r.Status == RequestStatus.Submitted || r.Status == RequestStatus.Approved))
        {
            actions.Add(new ProcurementActionDto
            {
                MaterialId = req.MaterialId,
                PartNumber = req.Material.PartNumber,
                MaterialName = req.Material.Name,
                ActionType = "OpenRequest",
                Summary = $"Request #{req.RequestId} — {req.Status} — qty {req.Quantity}",
                Quantity = req.Quantity,
                RelatedId = req.RequestId,
                ReorderPlaced = req.Material.ReorderPlaced,
                ReorderNote = req.Material.ReorderNote
            });
        }

        return actions.OrderBy(a => a.ActionType).ThenBy(a => a.PartNumber).ToList();
    }

    public async Task MarkReorderAsync(int materialId, MarkReorderDto dto, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(materialId, cancellationToken)
            ?? throw new NotFoundException($"Material {materialId} was not found.");

        material.ReorderPlaced = true;
        material.ReorderNote = dto.ReorderNote?.Trim();
        await _materialRepository.UpdateAsync(material, cancellationToken);
    }
}
