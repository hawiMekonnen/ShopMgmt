namespace ShopMgmt.Domain.Entities;

public class Material
{
    public int MaterialId { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AircraftTypes { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal MinStock { get; set; }
    public int? DefaultShopId { get; set; }
    public Shop? DefaultShop { get; set; }
    public bool ReorderPlaced { get; set; }
    public string? ReorderNote { get; set; }
    /// <summary>When true, technicians cannot see or order this material in catalog search.</summary>
    public bool HiddenFromTechnicians { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<MaterialUsage> Usages { get; set; } = new List<MaterialUsage>();
    public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
    public ICollection<MaterialRequest> Requests { get; set; } = new List<MaterialRequest>();
}
