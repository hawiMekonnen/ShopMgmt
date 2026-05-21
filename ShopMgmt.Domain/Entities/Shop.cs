namespace ShopMgmt.Domain.Entities;

public class Shop
{
    public int ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public ICollection<MaterialUsage> Usages { get; set; } = new List<MaterialUsage>();
}
