using AutoMapper;
using FluentValidation;
using Moq;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Mapping;
using ShopMgmt.Application.Services;
using ShopMgmt.Application.Validators;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;
using ShopMgmt.Infrastructure.Repositories;
using ShopMgmt.Tests.Infrastructure;

namespace ShopMgmt.Tests.Services;

public class MaterialServiceTests
{
    private static MaterialService CreateService(AppDbContext context)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MaterialMappingProfile>()).CreateMapper();
        var audit = new Mock<IAuditRecorder>();

        return new MaterialService(
            new MaterialRepository(context),
            new CategoryRepository(context),
            new StockBatchRepository(context),
            mapper,
            audit.Object,
            new CreateMaterialDtoValidator(),
            new UpdateMaterialDtoValidator());
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAt()
    {
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Consumables" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var result = await service.CreateAsync(new CreateMaterialDto
        {
            Name = "Grease",
            CategoryId = category.CategoryId,
            UnitPrice = 10m,
            Unit = "kg"
        });

        Assert.True(result.CreatedAt <= DateTime.UtcNow);
        Assert.True(result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task GetAllAsync_CalculatesOnHandAndStockValue()
    {
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Tools" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var material = new Material
        {
            Name = "Wrench",
            CategoryId = category.CategoryId,
            UnitPrice = 5m,
            Unit = "ea",
            CreatedAt = DateTime.UtcNow
        };
        context.Materials.Add(material);
        await context.SaveChangesAsync();

        context.StockBatches.Add(new StockBatch
        {
            MaterialId = material.MaterialId,
            QuantityReceived = 100m,
            ReceivedAt = DateTime.UtcNow,
            CostTotal = 500m
        });
        context.Shops.Add(new Shop { ShopId = 1, Name = "Main", Location = "ADD" });
        context.Users.Add(new User { UserId = 1, Name = "Test", Email = "t@test.com", Role = Domain.Enums.UserRole.Technician });
        context.MaterialUsages.Add(new MaterialUsage
        {
            MaterialId = material.MaterialId,
            ShopId = 1,
            QuantityUsed = 30m,
            UsedAt = DateTime.UtcNow,
            UserId = 1
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var list = await service.GetAllAsync();

        var item = Assert.Single(list);
        Assert.Equal(70m, item.OnHand);
        Assert.Equal(350m, item.StockValue);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsWhenUsagesExist()
    {
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Parts" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var material = new Material
        {
            Name = "Bolt",
            CategoryId = category.CategoryId,
            UnitPrice = 1m,
            Unit = "ea",
            CreatedAt = DateTime.UtcNow
        };
        context.Materials.Add(material);
        await context.SaveChangesAsync();

        context.Shops.Add(new Shop { ShopId = 1, Name = "Main", Location = "ADD" });
        context.Users.Add(new User { UserId = 1, Name = "Test", Email = "t@test.com", Role = Domain.Enums.UserRole.Technician });
        context.MaterialUsages.Add(new MaterialUsage
        {
            MaterialId = material.MaterialId,
            ShopId = 1,
            QuantityUsed = 1m,
            UsedAt = DateTime.UtcNow,
            UserId = 1
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);
        await Assert.ThrowsAsync<Application.Exceptions.ConflictException>(
            () => service.DeleteAsync(material.MaterialId));
    }
}
