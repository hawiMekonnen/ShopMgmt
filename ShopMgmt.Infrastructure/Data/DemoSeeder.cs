using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Data;

public static class DemoSeeder
{
    public const string DemoPassword = "Demo@123";

    public static async Task SeedAsync(IServiceProvider services, IHostEnvironment env)
    {
        if (!env.IsDevelopment())
            return;

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Demo seed skipped — database already has users.");
            return;
        }

        logger.LogInformation("Seeding AMOS-style demo data...");

        var radioShop = new Shop { Name = "Radio Shop" };
        var lineShop = new Shop { Name = "Line Maintenance Store" };
        context.Shops.AddRange(radioShop, lineShop);
        await context.SaveChangesAsync();

        var consumables = new Category { Name = "Avionics Consumables", Description = "Wire, connectors, LRU supplies" };
        var hardware = new Category { Name = "Hardware", Description = "Fasteners and fittings" };
        context.Categories.AddRange(consumables, hardware);
        await context.SaveChangesAsync();

        var wire = new Material
        {
            PartNumber = "ET-AVN-WIRE-22-10",
            Name = "Aircraft Wire 22AWG 10m Spool",
            Description = "MIL-spec hookup wire for avionics benches",
            AircraftTypes = "B737,B787",
            CategoryId = consumables.CategoryId,
            UnitPrice = 450m,
            Unit = "spool",
            MinStock = 5m,
            DefaultShopId = radioShop.ShopId,
            CreatedAt = DateTime.UtcNow
        };

        var connector = new Material
        {
            PartNumber = "ET-AVN-CONN-D38999",
            Name = "D38999 Connector Kit",
            Description = "Radio shop connector assortment",
            AircraftTypes = "B737",
            CategoryId = consumables.CategoryId,
            UnitPrice = 1200m,
            Unit = "kit",
            MinStock = 2m,
            DefaultShopId = radioShop.ShopId,
            CreatedAt = DateTime.UtcNow
        };

        context.Materials.AddRange(wire, connector);
        await context.SaveChangesAsync();

        context.StockBatches.AddRange(
            new StockBatch
            {
                MaterialId = wire.MaterialId,
                ShopId = radioShop.ShopId,
                QuantityReceived = 20m,
                ReceivedAt = DateTime.UtcNow.AddDays(-14),
                CostTotal = 9000m,
                Status = MaterialStatus.Serviceable
            },
            new StockBatch
            {
                MaterialId = wire.MaterialId,
                ShopId = radioShop.ShopId,
                QuantityReceived = 5m,
                ReceivedAt = DateTime.UtcNow.AddDays(-2),
                CostTotal = 2250m,
                Status = MaterialStatus.Pending
            },
            new StockBatch
            {
                MaterialId = connector.MaterialId,
                ShopId = radioShop.ShopId,
                QuantityReceived = 3m,
                ReceivedAt = DateTime.UtcNow.AddDays(-7),
                CostTotal = 3600m,
                Status = MaterialStatus.Serviceable,
                ExpiryDate = DateTime.UtcNow.AddDays(20)
            });

        var admin = new User
        {
            Name = "System Admin",
            Email = "admin@demo.et",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
            Role = UserRole.Admin
        };
        var tech = new User
        {
            Name = "Radio Technician",
            Email = "technician@demo.et",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
            Role = UserRole.Technician,
            ShopId = radioShop.ShopId
        };
        var manager = new User
        {
            Name = "Radio Shop Manager",
            Email = "shopmanager@demo.et",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
            Role = UserRole.ShopManager,
            ShopId = radioShop.ShopId
        };
        var procurement = new User
        {
            Name = "Procurement Officer",
            Email = "procurement@demo.et",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
            Role = UserRole.Procurement
        };

        context.Users.AddRange(admin, tech, manager, procurement);
        await context.SaveChangesAsync();

        var readyRequest = new MaterialRequest
        {
            MaterialId = wire.MaterialId,
            ShopId = radioShop.ShopId,
            RequestedByUserId = tech.UserId,
            Quantity = 2m,
            AircraftOrWorkOrder = "ET-AUE / WO-4401",
            Status = RequestStatus.ReadyForPickup,
            CreatedAt = DateTime.UtcNow.AddHours(-4),
            ApprovedAt = DateTime.UtcNow.AddHours(-3),
            ReadyAt = DateTime.UtcNow.AddHours(-1),
            Notes = "Urgent bench repair"
        };
        context.MaterialRequests.Add(readyRequest);
        await context.SaveChangesAsync();

        context.Alerts.Add(new Alert
        {
            MaterialId = wire.MaterialId,
            Type = AlertType.PickupReady,
            Threshold = 0,
            CurrentQuantity = readyRequest.Quantity,
            TriggeredAt = DateTime.UtcNow.AddHours(-1),
            CreatedBy = manager.UserId,
            RequestId = readyRequest.RequestId
        });

        await context.SaveChangesAsync();
        logger.LogInformation("Demo seed completed.");
    }
}
