using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Models;

public class MaterialListRow
{
    public Material Material { get; set; } = null!;
    public string CategoryName { get; set; } = string.Empty;
    public decimal OnHand { get; set; }
    public decimal StockValue { get; set; }
}
