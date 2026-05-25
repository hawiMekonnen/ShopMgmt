using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IMaterialRepository _materialRepository;
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public AlertService(
        IAlertRepository alertRepository,
        IAuditLogService auditLogService,
        IMaterialRepository materialRepository,
        IStockBatchRepository stockBatchRepository,
        IRealtimeNotifier realtimeNotifier)
    {
        _alertRepository = alertRepository;
        _auditLogService = auditLogService;
        _materialRepository = materialRepository;
        _stockBatchRepository = stockBatchRepository;
        _realtimeNotifier = realtimeNotifier;
    }

    private static AlertDto MapToDto(Alert alert) =>
        new()
        {
            AlertId = alert.AlertId,
            MaterialId = alert.MaterialId,
            MaterialName = alert.Material?.Name ?? string.Empty,
            Threshold = alert.Threshold,
            CurrentQuantity = alert.CurrentQuantity,
            TriggeredAt = alert.TriggeredAt,
            ResolvedAt = alert.ResolvedAt,
            ResolvedNote = alert.ResolvedNote,
            Type = alert.Type.ToString(),
            CreatedByName = alert.User?.Name ?? string.Empty,
            RequestId = alert.RequestId,
            Note = alert.Note,
            CreatedAt = alert.CreatedAt
        };

    public async Task<List<AlertDto>> GetActiveAlertsAsync()
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync();
        return alerts.Select(MapToDto).ToList();
    }

    public async Task<AlertDto> ResolveAlertAsync(int alertId, ResolveAlertDto dto)
    {
        var alert = await _alertRepository.GetByIdAsync(alertId);
        if (alert == null)
            throw new Exception($"Alert with ID {alertId} not found.");
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedNote = dto.ResolvedNote;
        await _alertRepository.UpdateAsync(alert);
        await _auditLogService.LogAsync("Update", "Alert", alert.AlertId, dto.ResolvedBy, $"Resolved alert with note: {dto.ResolvedNote}");
        return MapToDto(alert);
    }

    public async Task<AlertDto?> CreateNewMaterialAlertAsync(
        int materialId,
        string materialName,
        string partNumber,
        int createdBy,
        CancellationToken cancellationToken = default)
    {
        var exists = await _alertRepository.ActiveAlertExistsAsync(materialId, AlertType.NewMaterialAdded);
        if (exists)
            return null;

        var alert = await SaveAndNotifyAsync(new Alert
        {
            MaterialId = materialId,
            Type = AlertType.NewMaterialAdded,
            Threshold = 0,
            CurrentQuantity = 0,
            TriggeredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Note = $"New material '{materialName}' ({partNumber}) added to catalog."
        }, broadcastAlert: false, cancellationToken);

        await _realtimeNotifier.NotifyNewMaterialAsync(materialId, materialName, partNumber, cancellationToken);
        return alert;
    }

    public async Task<AlertDto?> CreatePickupReadyAlertAsync(
        int materialId,
        int requestId,
        decimal quantity,
        int createdBy,
        CancellationToken cancellationToken = default)
    {
        if (await _alertRepository.ActivePickupAlertExistsForRequestAsync(requestId))
            return null;

        return await SaveAndNotifyAsync(new Alert
        {
            MaterialId = materialId,
            Type = AlertType.PickupReady,
            Threshold = 0,
            CurrentQuantity = quantity,
            TriggeredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            RequestId = requestId,
            Note = $"Request #{requestId} is ready for pickup."
        }, cancellationToken: cancellationToken);
    }

    public async Task CheckAndCreateLowStockAlertsAsync()
    {
        var rows = await _materialRepository.GetAllWithInventoryAsync();

        foreach (var row in rows)
        {
            var material = row.Material;
            bool isLowStock = row.Available < material.MinStock && material.MinStock > 0;
            bool exists = await _alertRepository.ActiveAlertExistsAsync(material.MaterialId, AlertType.LowStock);

            if (isLowStock && !exists)
            {
                await SaveAndNotifyAsync(new Alert
                {
                    MaterialId = material.MaterialId,
                    Type = AlertType.LowStock,
                    Threshold = material.MinStock,
                    CurrentQuantity = row.Available,
                    TriggeredAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    Note = $"Low stock: {row.Available} {material.Unit} available (min {material.MinStock})."
                });
            }
            else if (!isLowStock && exists)
            {
                var activeAlerts = await _alertRepository.GetByMaterialAsync(material.MaterialId);
                var lowStockAlerts = activeAlerts.Where(a => a.Type == AlertType.LowStock && a.ResolvedAt == null);
                foreach (var alert in lowStockAlerts)
                {
                    alert.ResolvedAt = DateTime.UtcNow;
                    alert.ResolvedNote = $"Auto-resolved: Stock replenished to {row.Available} {material.Unit}.";
                    await _alertRepository.UpdateAsync(alert);
                    await _auditLogService.LogAsync("Update", "Alert", alert.AlertId, 1, $"Auto-resolved low stock alert: Stock replenished to {row.Available} {material.Unit}.");
                }
            }
        }
    }

    public async Task CheckAndCreateExpiryAlertsAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(30);
        var expiring = await _stockBatchRepository.GetExpiringBeforeAsync(cutoff);

        foreach (var batch in expiring)
        {
            bool exists = await _alertRepository.ActiveAlertExistsAsync(
                batch.MaterialId, AlertType.ExpiryWarning);

            if (!exists)
            {
                var expiryText = batch.ExpiryDate?.ToString("yyyy-MM-dd") ?? "unknown";
                await SaveAndNotifyAsync(new Alert
                {
                    MaterialId = batch.MaterialId,
                    Type = AlertType.ExpiryWarning,
                    Threshold = 0,
                    CurrentQuantity = batch.QuantityReceived,
                    TriggeredAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    Note = $"Batch #{batch.BatchId} expires on {expiryText} ({batch.QuantityReceived} units)."
                });
            }
        }
    }

    private async Task<AlertDto> SaveAndNotifyAsync(
        Alert alert,
        bool broadcastAlert = true,
        CancellationToken cancellationToken = default)
    {
        var saved = await _alertRepository.AddAsync(alert);
        var loaded = await _alertRepository.GetByIdAsync(saved.AlertId)
            ?? throw new InvalidOperationException("Alert was saved but could not be loaded.");
        var dto = MapToDto(loaded);
        if (broadcastAlert)
            await _realtimeNotifier.NotifyAlertCreatedAsync(dto, cancellationToken);
        return dto;
    }
}
