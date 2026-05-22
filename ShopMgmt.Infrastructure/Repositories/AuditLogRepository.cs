using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;
        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAllAsync(string? entity, int? performedBy, DateTime? from, DateTime? to, int page, int pageSize)
        {
            var query = _context.AuditLogs.Include(l => l.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(entity))
                query = query.Where(l => l.Entity == entity);
            if (performedBy.HasValue)
                query = query.Where(l => l.PerformedBy == performedBy.Value);
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);

            return await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? entity, int? performedBy, DateTime? from, DateTime? to)
        {
            var query = _context.AuditLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(entity))
                query = query.Where(l => l.Entity == entity);
            if (performedBy.HasValue)
                query = query.Where(l => l.PerformedBy == performedBy.Value);
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);

            return await query.CountAsync();
        }

        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
