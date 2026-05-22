namespace ShopMgmt.Application.DTOs;

public class ServiceabilityCheckDto
{
    public int CheckId { get; set; }
    public int BatchId { get; set; }
    public int TechnicianId { get; set; }
    public DateTime CheckedAt { get; set; }
    public bool Passed { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? Notes { get; set; }
}
