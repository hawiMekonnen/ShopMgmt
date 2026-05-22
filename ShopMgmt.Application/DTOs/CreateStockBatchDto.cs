namespace ShopMgmt.Application.DTOs;

public class CreateStockBatchDto
{
    public decimal QuantityReceived { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal CostTotal { get; set; }
    public int? ShopId { get; set; }
}
