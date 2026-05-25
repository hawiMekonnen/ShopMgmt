namespace ShopMgmt.Application.DTOs;

public class UserListItemDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? ShopId { get; set; }
}

public class CreateTechnicianDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class SetMaterialTechnicianVisibilityDto
{
    public bool HiddenFromTechnicians { get; set; }
}

public class ShopActivityDto
{
    public IReadOnlyList<MaterialRequestDto> Requests { get; set; } = Array.Empty<MaterialRequestDto>();
    public IReadOnlyList<MaterialUsageActivityDto> RecentUsages { get; set; } = Array.Empty<MaterialUsageActivityDto>();
    public IReadOnlyList<UserListItemDto> Technicians { get; set; } = Array.Empty<UserListItemDto>();
}

public class MaterialUsageActivityDto
{
    public int UsageId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal QuantityUsed { get; set; }
    public DateTime UsedAt { get; set; }
    public string? FlightNumber { get; set; }
}

public class RejectMaterialRequestDto
{
    public string? Notes { get; set; }
}
