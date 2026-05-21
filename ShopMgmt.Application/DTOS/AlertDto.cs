using System;
namespace ShopMgmt.Application.DTOS;
public class AlertDto
{
    public int AlertId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Threshold { get; set; }
    public decimal CurrentQuantity { get; set; }
    public DateTime TriggeredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedNote { get; set; }
    public string Type { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
}
