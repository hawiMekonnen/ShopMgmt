using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IMaterialRepository _materialRepository;
    private readonly IStockBatchRepository _stockBatchRepository;

    public AlertService(
        IAlertRepository alertRepository,
        IAuditLogService auditLogService,
        IMaterialRepository materialRepository,
        IStockBatchRepository stockBatchRepository)
    {
        _alertRepository = alertRepository;
        _auditLogService = auditLogService;
        _materialRepository = materialRepository;
        _stockBatchRepository = stockBatchRepository;
    }

    private static AlertDto MapToDto(Alert alert)
    {
        return new AlertDto
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
            CreatedByName = alert.User?.Name ?? string.Empty
        };
    }

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

    public async Task CheckAndCreateLowStockAlertsAsync()
    {
        const decimal threshold = 10m;
        var rows = await _materialRepository.GetAllWithInventoryAsync();

        foreach (var row in rows.Where(r => r.OnHand < threshold))
        {
            bool exists = await _alertRepository.ActiveAlertExistsAsync(
                row.Material.MaterialId, AlertType.LowStock);

            if (!exists)
            {
                await _alertRepository.AddAsync(new Alert
                {
                    MaterialId = row.Material.MaterialId,
                    Type = AlertType.LowStock,
                    Threshold = threshold,
                    CurrentQuantity = row.OnHand,
                    TriggeredAt = DateTime.UtcNow,
                    CreatedBy = 1
                });
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
                await _alertRepository.AddAsync(new Alert
                {
                    MaterialId = batch.MaterialId,
                    Type = AlertType.ExpiryWarning,
                    Threshold = 0,
                    CurrentQuantity = batch.QuantityReceived,
                    TriggeredAt = DateTime.UtcNow,
                    CreatedBy = 1
                });
            }
        }
    }
}
