namespace ShopMgmt.Application.Models;

public class MaterialInventorySnapshot
{
    public int MaterialId { get; set; }
    public decimal OnHand { get; set; }
    public decimal Blocked { get; set; }
    public decimal Reserved { get; set; }
    public decimal Available { get; set; }
    public decimal StockValue { get; set; }
}
