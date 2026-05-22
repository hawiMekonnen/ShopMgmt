using ShopMgmt.Application.Interfaces;

namespace ShopMgmt.Infrastructure.Audit;

public class NoOpAuditRecorder : IAuditRecorder
{
    public Task RecordAsync(string action, string entity, int entityId, string details, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
