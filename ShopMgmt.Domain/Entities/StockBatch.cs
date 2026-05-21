namespace ShopMgmt.Domain.Entities;

public class StockBatch
{
    public int BatchId { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public decimal QuantityReceived { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal CostTotal { get; set; }
}
