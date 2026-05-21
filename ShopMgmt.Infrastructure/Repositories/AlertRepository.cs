using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AppDbContext _context;

        public AlertRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alert>> GetActiveAlertsAsync()
        {
            return await _context.Alerts
                .Include(a => a.Material)
                .Include(a => a.User)
                .Where(a => a.ResolvedAt == null)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();
        }

        public async Task<List<Alert>> GetByMaterialAsync(int materialId)
        {
            return await _context.Alerts
                .Include(a => a.Material)
                .Include(a => a.User)
                .Where(a => a.MaterialId == materialId && a.ResolvedAt == null)
                .ToListAsync();
        }

        public async Task<Alert?> GetByIdAsync(int alertId)
        {
            return await _context.Alerts
                .Include(a => a.Material)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AlertId == alertId);
        }

        public async Task<Alert> AddAsync(Alert alert)
        {
            await _context.Alerts.AddAsync(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task UpdateAsync(Alert alert)
        {
            _context.Alerts.Update(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ActiveAlertExistsAsync(int materialId, AlertType type)
        {
            return await _context.Alerts
                .AnyAsync(a => a.MaterialId == materialId && a.Type == type && a.ResolvedAt == null);
        }
    }
}
