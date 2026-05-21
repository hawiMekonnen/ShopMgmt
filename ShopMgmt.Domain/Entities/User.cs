using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public ICollection<MaterialUsage> Usages { get; set; } = new List<MaterialUsage>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
