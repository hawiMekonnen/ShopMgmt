using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Services;
using ShopMgmt.Infrastructure.Repositories;
using ShopMgmt.Infrastructure.Services;
using ShopMgmt.Application;
using ShopMgmt.Infrastructure;
using ShopMgmt.Infrastructure.BackgroundServices;
using ShopMgmt.WebAPI.Middleware;
using ShopMgmt.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using ShopMgmt.Application.Validators;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Infrastructure.BackgroundServices;
using ShopMgmt.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateMaterialDtoValidator>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// Role-based policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireShopManager", policy => policy.RequireRole("ShopManager"));
    options.AddPolicy("RequireTechnician", policy => policy.RequireRole("Technician"));
    options.AddPolicy("RequireProcurement", policy => policy.RequireRole("Procurement"));
    options.AddPolicy("RequireFinance", policy => policy.RequireRole("Finance"));
});

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
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IServiceabilityCheckRepository, ServiceabilityCheckRepository>();
// Register Services
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IMaterialUsageService, MaterialUsageService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IServiceabilityCheckService, ServiceabilityCheckService>();

// Register Background Services
builder.Services.AddHostedService<AlertMonitorService>();

var app = builder.Build();

await DemoSeeder.SeedAsync(app.Services, app.Environment);

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("SpaClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
