namespace ShopMgmt.Application.DTOs;

public class UpdateMaterialDto
{
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
}
