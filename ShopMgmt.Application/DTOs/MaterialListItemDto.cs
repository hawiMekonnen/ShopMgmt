namespace ShopMgmt.Application.DTOs;

public class MaterialListItemDto
{
    public int MaterialId { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AircraftTypes { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal MinStock { get; set; }
    public decimal OnHand { get; set; }
    public decimal Blocked { get; set; }
    public decimal Reserved { get; set; }
    public decimal Available { get; set; }
    public decimal StockValue { get; set; }
    public int? DefaultShopId { get; set; }
    public bool ReorderPlaced { get; set; }
    public string? ReorderNote { get; set; }
}
