namespace ShopMgmt.Application.DTOs;

public class UpdateMaterialDto
{
    public string PartNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AircraftTypes { get; set; }
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal MinStock { get; set; }
    public int? DefaultShopId { get; set; }
}
