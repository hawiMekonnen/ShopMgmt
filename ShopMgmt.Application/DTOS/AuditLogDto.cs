using System;

namespace ShopMgmt.Application.DTOS;

public class AuditLogDto
{
    public int LogId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int PerformedBy { get; set; }
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}
