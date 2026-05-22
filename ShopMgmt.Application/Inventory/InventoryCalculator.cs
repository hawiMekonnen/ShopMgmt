using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Application.Inventory;

public static class InventoryCalculator
{
    public static (decimal OnHand, decimal Blocked, decimal Available, decimal StockValue) Compute(
        decimal serviceableReceived,
        decimal pendingReceived,
        decimal quarantinedReceived,
        decimal condemnedReceived,
        decimal totalUsed,
        decimal reserved,
        decimal unitPrice)
    {
        var countableReceived = serviceableReceived + pendingReceived;
        var onHand = Math.Max(0, countableReceived - totalUsed);
        var blocked = pendingReceived;
        var available = Math.Max(0, serviceableReceived - totalUsed - reserved);
        var stockValue = available * unitPrice;
        _ = quarantinedReceived;
        _ = condemnedReceived;
        return (onHand, blocked, available, stockValue);
    }

    public static bool IsIssuableStatus(MaterialStatus status) =>
        status == MaterialStatus.Serviceable;
}
