namespace ShopMgmt.Application.DTOs;

public class StockBatchDto
{
    public int BatchId { get; set; }
    public int MaterialId { get; set; }
    public decimal QuantityReceived { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal CostTotal { get; set; }
}
