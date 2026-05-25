using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Domain.Entities;

public class Alert
{
    public int AlertId { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public decimal Threshold { get; set; }
    public decimal CurrentQuantity { get; set; }
    public DateTime TriggeredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int CreatedBy { get; set; }
    public User User { get; set; } = null!;
    public AlertType Type { get; set; }
    public string? ResolvedNote { get; set; }
    public int? RequestId { get; set; }
    public MaterialRequest? Request { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Note { get; set; }
}
