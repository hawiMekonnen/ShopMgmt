using Moq;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Services;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Repositories;
using ShopMgmt.Tests.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ShopMgmt.Tests.Services;

public class ServiceabilityCheckServiceTests
{
    [Fact]
    public async Task RecordCheckAsync_Passed_UpdatesBatchToServiceable()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Avionics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var material = new Material
        {
            Name = "Transponder",
            CategoryId = category.CategoryId,
            UnitPrice = 500m,
            Unit = "Units",
            CreatedAt = DateTime.UtcNow
        };
        context.Materials.Add(material);
        await context.SaveChangesAsync();

        var batch = new StockBatch
        {
            MaterialId = material.MaterialId,
            QuantityReceived = 10m,
            CostTotal = 5000m,
            ReceivedAt = DateTime.UtcNow,
            Status = MaterialStatus.Pending
        };
        context.StockBatches.Add(batch);
        await context.SaveChangesAsync();

        var checkRepository = new ServiceabilityCheckRepository(context);
        var stockBatchRepository = new StockBatchRepository(context);
        var service = new ServiceabilityCheckService(checkRepository, stockBatchRepository);

        var dto = new CreateServiceabilityCheckDto
        {
            BatchId = batch.BatchId,
            TechnicianId = 1,
            Passed = true,
            ReferenceDocument = "ATA 34-10-01 Rev 2",
            Notes = "All parameters within standard limits."
        };

        // Act
        var result = await service.RecordCheckAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Passed);
        Assert.Equal("ATA 34-10-01 Rev 2", result.ReferenceDocument);

        var updatedBatch = await stockBatchRepository.GetByIdAsync(batch.BatchId);
        Assert.NotNull(updatedBatch);
        Assert.Equal(MaterialStatus.Serviceable, updatedBatch.Status);
        Assert.Null(updatedBatch.QuarantineDate);
        Assert.Null(updatedBatch.QuarantineReason);

        var history = await service.GetHistoryByBatchAsync(batch.BatchId);
        Assert.Single(history);
        Assert.Equal("ATA 34-10-01 Rev 2", history[0].ReferenceDocument);
    }

    [Fact]
    public async Task RecordCheckAsync_Failed_UpdatesBatchToQuarantined()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var category = new Category { Name = "Avionics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var material = new Material
        {
            Name = "Transponder",
            CategoryId = category.CategoryId,
            UnitPrice = 500m,
            Unit = "Units",
            CreatedAt = DateTime.UtcNow
        };
        context.Materials.Add(material);
        await context.SaveChangesAsync();

        var batch = new StockBatch
        {
            MaterialId = material.MaterialId,
            QuantityReceived = 10m,
            CostTotal = 5000m,
            ReceivedAt = DateTime.UtcNow,
            Status = MaterialStatus.Pending
        };
        context.StockBatches.Add(batch);
        await context.SaveChangesAsync();

        var checkRepository = new ServiceabilityCheckRepository(context);
        var stockBatchRepository = new StockBatchRepository(context);
        var service = new ServiceabilityCheckService(checkRepository, stockBatchRepository);

        var dto = new CreateServiceabilityCheckDto
        {
            BatchId = batch.BatchId,
            TechnicianId = 2,
            Passed = false,
            ReferenceDocument = "ATA 34-10-01 Rev 2",
            Notes = "Frequency drift detected."
        };

        // Act
        var result = await service.RecordCheckAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Passed);
        Assert.Equal("Frequency drift detected.", result.Notes);

        var updatedBatch = await stockBatchRepository.GetByIdAsync(batch.BatchId);
        Assert.NotNull(updatedBatch);
        Assert.Equal(MaterialStatus.Quarantined, updatedBatch.Status);
        Assert.NotNull(updatedBatch.QuarantineDate);
        Assert.Equal("Frequency drift detected.", updatedBatch.QuarantineReason);
    }
}
