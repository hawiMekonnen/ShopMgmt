namespace ShopMgmt.Application.DTOs;

public class ProcurementPurchaseDto
{
    public int BatchId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int? ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public decimal QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostTotal { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}

public class ShopSpendSummaryDto
{
    public int? ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public decimal TotalQuantity { get; set; }
}

public class ProcurementBudgetReportDto
{
    public decimal TotalSpent { get; set; }
    public decimal MonthlySpent { get; set; }
    public decimal TotalQuantityPurchased { get; set; }
    public IReadOnlyList<ShopSpendSummaryDto> ByShop { get; set; } = Array.Empty<ShopSpendSummaryDto>();
    public IReadOnlyList<ProcurementPurchaseDto> Purchases { get; set; } = Array.Empty<ProcurementPurchaseDto>();
}
