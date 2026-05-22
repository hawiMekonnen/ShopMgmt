using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.DTOs;

public class StockBatchDto
{
    public int BatchId { get; set; }
    public int MaterialId { get; set; }
    public int? ShopId { get; set; }
    public decimal QuantityReceived { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal CostTotal { get; set; }
    public MaterialStatus Status { get; set; }
    public DateTime? QuarantineDate { get; set; }
    public string? QuarantineReason { get; set; }
}
