using ShopMgmt.Application.Inventory;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Tests.Inventory;

public class InventoryCalculatorTests
{
    [Fact]
    public void Compute_ServiceableOnly_AvailableAfterUsage()
    {
        var (onHand, blocked, available, _) = InventoryCalculator.Compute(
            serviceableReceived: 100m,
            pendingReceived: 0m,
            quarantinedReceived: 0m,
            condemnedReceived: 0m,
            totalUsed: 30m,
            reserved: 0m,
            unitPrice: 5m);

        Assert.Equal(70m, onHand);
        Assert.Equal(0m, blocked);
        Assert.Equal(70m, available);
    }

    [Fact]
    public void Compute_PendingBatch_BlockedNotAvailable()
    {
        var (onHand, blocked, available, _) = InventoryCalculator.Compute(
            serviceableReceived: 0m,
            pendingReceived: 50m,
            quarantinedReceived: 0m,
            condemnedReceived: 0m,
            totalUsed: 0m,
            reserved: 0m,
            unitPrice: 1m);

        Assert.Equal(50m, onHand);
        Assert.Equal(50m, blocked);
        Assert.Equal(0m, available);
    }

    [Fact]
    public void Compute_Reservation_ReducesAvailable()
    {
        var (_, _, available, _) = InventoryCalculator.Compute(
            serviceableReceived: 100m,
            pendingReceived: 0m,
            quarantinedReceived: 0m,
            condemnedReceived: 0m,
            totalUsed: 10m,
            reserved: 25m,
            unitPrice: 1m);

        Assert.Equal(65m, available);
    }

    [Fact]
    public void IsIssuableStatus_OnlyServiceable()
    {
        Assert.True(InventoryCalculator.IsIssuableStatus(MaterialStatus.Serviceable));
        Assert.False(InventoryCalculator.IsIssuableStatus(MaterialStatus.Pending));
        Assert.False(InventoryCalculator.IsIssuableStatus(MaterialStatus.Quarantined));
    }
}
