using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Repositories;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Infrastructure.Audit;

public class DbAuditRecorder : IAuditRecorder
{
    private readonly IAuditLogRepository _repository;

    public DbAuditRecorder(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task RecordAsync(string action, string entity, int entityId,
        string details, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(new AuditLog
        {
            Action = action,
            Entity = entity,
            EntityId = entityId,
            PerformedBy = 1, // system user — update when JWT userId threading is complete
            Details = details,
            Timestamp = DateTime.UtcNow
        });
    }
}
