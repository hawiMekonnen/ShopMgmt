using System;

namespace ShopMgmt.Application.DTOS;

public class MaterialUsageDto
{
    public int UsageId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public int ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public decimal QuantityUsed { get; set; }
    public DateTime DateUsed { get; set; }
    public string? FlightNumber { get; set; }
    public int UserId { get; set; }
}
