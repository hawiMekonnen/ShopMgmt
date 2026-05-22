using ShopMgmt.Application.DTOS;
using System;
using System.Threading.Tasks;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IAuditLogService
{
    Task LogAsync(string action, string entity, int entityId, int performedBy, string details);
    Task<AuditLogPagedDto> GetLogsAsync(string? entity, int? performedBy, DateTime? from, DateTime? to, int page = 1, int pageSize = 50);
}
