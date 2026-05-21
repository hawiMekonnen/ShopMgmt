using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Infrastructure.Audit;
using ShopMgmt.Infrastructure.Context;
using ShopMgmt.Infrastructure.Repositories;

namespace ShopMgmt.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=ShopMgmtDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IStockBatchRepository, StockBatchRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditRecorder, DbAuditRecorder>();

        return services;
    }
}
