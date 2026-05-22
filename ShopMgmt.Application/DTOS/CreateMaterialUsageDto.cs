using System;
using System.ComponentModel.DataAnnotations;

namespace ShopMgmt.Application.DTOS;

public class CreateMaterialUsageDto
{
    [Required]
    public int MaterialId { get; set; }
    
    [Required]
    public int ShopId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity used must be greater than zero.")]
    public decimal QuantityUsed { get; set; }
    
    public string? TailNumber { get; set; }
    
    [Required]
    public int UserId { get; set; }
}
