using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ShopMgmt.Application.Interfaces.Services;

namespace ShopMgmt.Infrastructure.BackgroundServices
{
    public class AlertMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(30);

        public AlertMonitorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                await alertService.CheckAndCreateLowStockAlertsAsync();
                await alertService.CheckAndCreateExpiryAlertsAsync();

                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}
