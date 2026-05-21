namespace ShopMgmt.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalMaterials { get; set; }
    public int TotalCategories { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockCount { get; set; }
}
