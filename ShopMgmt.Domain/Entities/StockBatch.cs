using ShopMgmt.Domain.Enums;
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
    public MaterialStatus Status { get; set; } = MaterialStatus.Pending;
    public DateTime? QuarantineDate { get; set; }
    public string? QuarantineReason { get; set; }
}
