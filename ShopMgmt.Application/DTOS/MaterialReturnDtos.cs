namespace ShopMgmt.Application.DTOs;

public class CreateMaterialReturnDto
{
    public int MaterialId { get; set; }
    public int ShopId { get; set; }
    public int? UsageId { get; set; }
    public int? BatchId { get; set; }
    public decimal Quantity { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

public class MaterialReturnDto
{
    public int ReturnId { get; set; }
    public int MaterialId { get; set; }
    public int ShopId { get; set; }
    public int? UsageId { get; set; }
    public int? BatchId { get; set; }
    public decimal Quantity { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime ReturnedAt { get; set; }
}

public class ProcurementActionDto
{
    public int MaterialId { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public int? RelatedId { get; set; }
    public bool ReorderPlaced { get; set; }
    public string? ReorderNote { get; set; }
}

public class MarkReorderDto
{
    public string? ReorderNote { get; set; }
}

public class MarkReadyDto
{
    public string? Notes { get; set; }
}
