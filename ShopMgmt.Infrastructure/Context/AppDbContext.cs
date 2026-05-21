using Microsoft.EntityFrameworkCore;
using ShopMgmt.Domain.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Primary Keys
        modelBuilder.Entity<MaterialUsage>().HasKey(mu => mu.UsageId);
        modelBuilder.Entity<StockBatch>().HasKey(sb => sb.BatchId);
        modelBuilder.Entity<AuditLog>().HasKey(al => al.LogId);

        // Precision configurations
        modelBuilder.Entity<Material>()
            .Property(m => m.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<MaterialUsage>()
            .Property(mu => mu.QuantityUsed)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<StockBatch>()
            .Property(sb => sb.QuantityReceived)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<StockBatch>()
            .Property(sb => sb.CostTotal)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Alert>()
            .Property(a => a.Threshold)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Alert>()
            .Property(a => a.CurrentQuantity)
            .HasColumnType("decimal(18,2)");

        // Relationships
        modelBuilder.Entity<MaterialUsage>()
            .HasOne(mu => mu.User)
            .WithMany(u => u.Usages)
            .HasForeignKey(mu => mu.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.PerformedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Alert>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
