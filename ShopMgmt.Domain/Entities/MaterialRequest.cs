using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Domain.Entities;

public class MaterialRequest
{
    public int RequestId { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    public int RequestedByUserId { get; set; }
    public User RequestedBy { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? AircraftOrWorkOrder { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Submitted;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? IssuedAt { get; set; }
}
