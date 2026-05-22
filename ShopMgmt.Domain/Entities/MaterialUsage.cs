namespace ShopMgmt.Domain.Entities;

public class MaterialUsage
{
    public int UsageId { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    public decimal QuantityUsed { get; set; }
    public DateTime UsedAt { get; set; }
    public string? TailNumber { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int? RequestId { get; set; }
    public MaterialRequest? Request { get; set; }
    public int? IssuedByUserId { get; set; }
    public int? CollectedByUserId { get; set; }
}
