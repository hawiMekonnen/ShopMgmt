using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(string action, string entity, int entityId, int performedBy, string details)
    {
        var log = new AuditLog
        {
            Action = action,
            Entity = entity,
            EntityId = entityId,
            PerformedBy = performedBy,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
        await _auditLogRepository.AddAsync(log);
    }

    public async Task<AuditLogPagedDto> GetLogsAsync(string? entity, int? performedBy, DateTime? from, DateTime? to, int page = 1, int pageSize = 50)
    {
        var logs = await _auditLogRepository.GetAllAsync(entity, performedBy, from, to, page, pageSize);
        var totalCount = await _auditLogRepository.CountAsync(entity, performedBy, from, to);
        var items = logs.Select(l => new AuditLogDto
        {
            LogId = l.LogId,
            Action = l.Action,
            Entity = l.Entity,
            EntityId = l.EntityId,
            PerformedBy = l.PerformedBy,
            PerformedByName = l.User?.Name ?? string.Empty,
            Timestamp = l.Timestamp,
            Details = l.Details
        }).ToList();

        return new AuditLogPagedDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0
        };
    }
}
