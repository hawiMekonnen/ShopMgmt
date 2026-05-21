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
            entity.Property(m => m.Name).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Unit).IsRequired().HasMaxLength(50);
            entity.Property(m => m.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(m => m.Category)
                .WithMany(c => c.Materials)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockBatch>(entity =>
        {
            entity.HasKey(sb => sb.BatchId);
            entity.Property(sb => sb.QuantityReceived).HasColumnType("decimal(18,2)");
            entity.Property(sb => sb.CostTotal).HasColumnType("decimal(18,2)");

            entity.HasOne(sb => sb.Material)
                .WithMany(m => m.StockBatches)
                .HasForeignKey(sb => sb.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MaterialUsage>(entity =>
        {
            entity.HasKey(mu => mu.UsageId);
            entity.Property(mu => mu.QuantityUsed).HasColumnType("decimal(18,2)");

            entity.HasOne(mu => mu.User)
                .WithMany(u => u.Usages)
                .HasForeignKey(mu => mu.UserId)
                .OnDelete(DeleteBehavior.Restrict);
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
        });
    }
}
