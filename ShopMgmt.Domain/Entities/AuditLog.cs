namespace ShopMgmt.Domain.Entities;

public class AuditLog
{
    public int LogId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int PerformedBy { get; set; }
    public User User { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}
