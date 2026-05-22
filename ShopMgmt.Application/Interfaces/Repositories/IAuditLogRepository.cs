using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Interfaces.Repositories;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetAllAsync(string? entity, int? performedBy, DateTime? from, DateTime? to, int page, int pageSize);
    Task<int> CountAsync(string? entity, int? performedBy, DateTime? from, DateTime? to);
    Task AddAsync(AuditLog log);
}
