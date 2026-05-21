namespace ShopMgmt.Application.DTOs;

public class MaterialDetailDto
{
    public int MaterialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal OnHand { get; set; }
    public decimal StockValue { get; set; }
    public IReadOnlyList<StockBatchDto> RecentBatches { get; set; } = Array.Empty<StockBatchDto>();
}
