using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.DTOs;

public class MaterialRequestDto
{
    public int RequestId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public int RequestedByUserId { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? AircraftOrWorkOrder { get; set; }
    public RequestStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? IssuedAt { get; set; }
}

public class CreateMaterialRequestDto
{
    public int MaterialId { get; set; }
    public int ShopId { get; set; }
    public decimal Quantity { get; set; }
    public string? AircraftOrWorkOrder { get; set; }
    public string? Notes { get; set; }
}

public class IssueMaterialRequestDto
{
    public int CollectedByUserId { get; set; }
    public string? FlightNumber { get; set; }
}

public class CancelMaterialRequestDto
{
    public string? Notes { get; set; }
}
