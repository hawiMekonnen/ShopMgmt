using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Application.Mapping;
using ShopMgmt.Application.Services;
using ShopMgmt.Application.Validators;

namespace ShopMgmt.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MaterialMappingProfile));
        services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<IStockBatchService, StockBatchService>();
        services.AddScoped<IMaterialRequestService, MaterialRequestService>();
        services.AddScoped<IMaterialReturnService, MaterialReturnService>();
        services.AddScoped<IProcurementService, ProcurementService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
