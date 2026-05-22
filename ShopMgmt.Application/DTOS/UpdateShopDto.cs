using System.ComponentModel.DataAnnotations;

namespace ShopMgmt.Application.DTOS;

public class UpdateShopDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
