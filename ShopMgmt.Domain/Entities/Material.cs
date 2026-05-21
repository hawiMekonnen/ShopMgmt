namespace ShopMgmt.Domain.Entities;

public class Material
{
    public int MaterialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<MaterialUsage> Usages { get; set; } = new List<MaterialUsage>();
    public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
}
