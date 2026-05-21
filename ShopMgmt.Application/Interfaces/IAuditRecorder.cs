namespace ShopMgmt.Application.Interfaces;

public interface IAuditRecorder
{
    Task RecordAsync(string action, string entity, int entityId, string details, CancellationToken cancellationToken = default);
}
