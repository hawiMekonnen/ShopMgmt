using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Application.Services;
using ShopMgmt.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ShopMgmt.Infrastructure.Context.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=ShopMgmtDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Register Repositories
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IMaterialUsageRepository, MaterialUsageRepository>();

// Register Services
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IMaterialUsageService, MaterialUsageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
