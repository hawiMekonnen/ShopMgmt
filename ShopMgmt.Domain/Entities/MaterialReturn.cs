namespace ShopMgmt.Domain.Entities;

public class MaterialReturn
{
    public int ReturnId { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    public int? UsageId { get; set; }
    public MaterialUsage? Usage { get; set; }
    public int? BatchId { get; set; }
    public StockBatch? Batch { get; set; }
    public int ReturnedByUserId { get; set; }
    public User ReturnedBy { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime ReturnedAt { get; set; }
}
