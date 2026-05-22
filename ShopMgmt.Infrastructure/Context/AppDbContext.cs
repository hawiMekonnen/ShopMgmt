using Microsoft.EntityFrameworkCore;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;

namespace ShopMgmt.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Material> Materials { get; set; } = null!;
    public DbSet<Shop> Shops { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<MaterialUsage> MaterialUsages { get; set; } = null!;
    public DbSet<StockBatch> StockBatches { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Alert> Alerts { get; set; } = null!;
    public DbSet<ServiceabilityCheck> ServiceabilityChecks { get; set; } = null!;
    public DbSet<MaterialRequest> MaterialRequests { get; set; } = null!;
    public DbSet<MaterialReturn> MaterialReturns { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.CategoryId);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(m => m.MaterialId);
            entity.Property(m => m.PartNumber).IsRequired().HasMaxLength(100);
            entity.HasIndex(m => m.PartNumber).IsUnique();
            entity.Property(m => m.Name).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Description).HasMaxLength(1000);
            entity.Property(m => m.AircraftTypes).HasMaxLength(500);
            entity.Property(m => m.Unit).IsRequired().HasMaxLength(50);
            entity.Property(m => m.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(m => m.MinStock).HasColumnType("decimal(18,2)");
            entity.Property(m => m.ReorderNote).HasMaxLength(500);

            entity.HasOne(m => m.Category)
                .WithMany(c => c.Materials)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.DefaultShop)
                .WithMany()
                .HasForeignKey(m => m.DefaultShopId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StockBatch>(entity =>
        {
            entity.HasKey(sb => sb.BatchId);
            entity.Property(sb => sb.QuantityReceived).HasColumnType("decimal(18,2)");
            entity.Property(sb => sb.CostTotal).HasColumnType("decimal(18,2)");
            entity.Property(sb => sb.Status).HasConversion<string>();
            entity.HasIndex(sb => sb.Status);

            entity.HasOne(sb => sb.Material)
                .WithMany(m => m.StockBatches)
                .HasForeignKey(sb => sb.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sb => sb.Shop)
                .WithMany()
                .HasForeignKey(sb => sb.ShopId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MaterialUsage>(entity =>
        {
            entity.HasKey(mu => mu.UsageId);
            entity.Property(mu => mu.QuantityUsed).HasColumnType("decimal(18,2)");

            entity.HasOne(mu => mu.User)
                .WithMany(u => u.Usages)
                .HasForeignKey(mu => mu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mu => mu.Request)
                .WithMany()
                .HasForeignKey(mu => mu.RequestId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MaterialRequest>(entity =>
        {
            entity.HasKey(r => r.RequestId);
            entity.Property(r => r.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(r => r.AircraftOrWorkOrder).HasMaxLength(100);
            entity.Property(r => r.Status).HasConversion<string>();
            entity.Property(r => r.Notes).HasMaxLength(500);

            entity.HasOne(r => r.Material)
                .WithMany(m => m.Requests)
                .HasForeignKey(r => r.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Shop)
                .WithMany()
                .HasForeignKey(r => r.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.RequestedBy)
                .WithMany()
                .HasForeignKey(r => r.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MaterialReturn>(entity =>
        {
            entity.HasKey(r => r.ReturnId);
            entity.Property(r => r.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(r => r.Remarks).IsRequired().HasMaxLength(1000);

            entity.HasOne(r => r.Material)
                .WithMany()
                .HasForeignKey(r => r.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Batch)
                .WithMany()
                .HasForeignKey(r => r.BatchId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(al => al.LogId);

            entity.HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.Property(a => a.Type).HasConversion<string>();
            entity.Property(a => a.Threshold).HasColumnType("decimal(18,2)");
            entity.Property(a => a.CurrentQuantity).HasColumnType("decimal(18,2)");

            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Request)
                .WithMany()
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ServiceabilityCheck>(entity =>
        {
            entity.HasKey(sc => sc.CheckId);

            entity.HasOne(sc => sc.Batch)
                .WithMany()
                .HasForeignKey(sc => sc.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sc => sc.Technician)
                .WithMany()
                .HasForeignKey(sc => sc.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
