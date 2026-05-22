using AutoMapper;
using Moq;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Mapping;
using ShopMgmt.Application.Services;
using ShopMgmt.Application.Validators;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Repositories;
using ShopMgmt.Tests.Infrastructure;

namespace ShopMgmt.Tests.Services;

public class StockBatchServiceTests
{
    [Fact]
    public async Task ReceiveAsync_IncreasesOnHand()
    {
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Fluid" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var material = new Material
        {
            PartNumber = "PN-OIL",
            Name = "Oil",
            CategoryId = category.CategoryId,
            UnitPrice = 20m,
            Unit = "L",
            CreatedAt = DateTime.UtcNow
        };
        context.Materials.Add(material);
        await context.SaveChangesAsync();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MaterialMappingProfile>()).CreateMapper();
        var audit = new Mock<IAuditRecorder>();

        var batchService = new StockBatchService(
            new StockBatchRepository(context),
            new MaterialRepository(context),
            mapper,
            audit.Object,
            new CreateStockBatchDtoValidator());

        await batchService.ReceiveAsync(material.MaterialId, new CreateStockBatchDto
        {
            QuantityReceived = 50m,
            ReceivedAt = DateTime.UtcNow,
            CostTotal = 1000m
        });

        var materialRepo = new MaterialRepository(context);
        var inventory = await materialRepo.GetInventoryAsync(material.MaterialId);
        Assert.NotNull(inventory);
        Assert.Equal(50m, inventory.OnHand);
        Assert.Equal(0m, inventory.Available);
        Assert.Equal(50m, inventory.Blocked);
    }
}
