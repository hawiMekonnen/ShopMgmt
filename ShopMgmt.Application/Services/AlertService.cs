using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IAuditLogService _auditLogService;

    public AlertService(IAlertRepository alertRepository, IAuditLogService auditLogService)
    {
        _alertRepository = alertRepository;
        _auditLogService = auditLogService;
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
            Type = alert.Type.ToString()
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
        // Iterate all materials and check on‑hand quantity
        // Assuming a Material repository exists in the system (not part of Member 3). Here we fetch via context directly.
        // Placeholder implementation – actual material access should be added later.
        // This method will be invoked by the background service.
    }

    public async Task CheckAndCreateExpiryAlertsAsync()
    {
        // Placeholder for expiry check logic – similar to low‑stock.
    }
}
