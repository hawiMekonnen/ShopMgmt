using System.ComponentModel.DataAnnotations;

namespace ShopMgmt.Application.DTOS;

public class CreateServiceabilityCheckDto
{
    [Required]
    public int BatchId { get; set; }

    [Required]
    public int TechnicianId { get; set; }

    [Required]
    public bool Passed { get; set; }

    public string? ReferenceDocument { get; set; }

    public string? Notes { get; set; }
}
