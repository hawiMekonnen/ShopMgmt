using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Application.Services;
using ShopMgmt.Infrastructure.Repositories;
using ShopMgmt.Application;
using ShopMgmt.Infrastructure;
using ShopMgmt.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Register Repositories
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IMaterialUsageRepository, MaterialUsageRepository>();

// Register Services
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IMaterialUsageService, MaterialUsageService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("SpaClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
