using AutoMapper;
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

public class CategoryServiceTests
{
    private static CategoryService CreateService(AppDbContext context)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MaterialMappingProfile>()).CreateMapper();
        var audit = new Mock<IAuditRecorder>();

        return new CategoryService(
            new CategoryRepository(context),
            mapper,
            audit.Object,
            new CreateCategoryDtoValidator(),
            new UpdateCategoryDtoValidator());
    }

    [Fact]
    public async Task DeleteAsync_ThrowsWhenMaterialsExist()
    {
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Electrical" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Materials.Add(new Material
        {
            Name = "Wire",
            CategoryId = category.CategoryId,
            UnitPrice = 2m,
            Unit = "m",
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);
        await Assert.ThrowsAsync<Application.Exceptions.ConflictException>(
            () => service.DeleteAsync(category.CategoryId));
    }
}
