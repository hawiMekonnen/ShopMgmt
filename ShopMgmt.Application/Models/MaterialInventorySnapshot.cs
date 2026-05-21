namespace ShopMgmt.Application.Models;

public class MaterialInventorySnapshot
{
    public int MaterialId { get; set; }
    public decimal OnHand { get; set; }
    public decimal StockValue { get; set; }
}
