namespace TaskBridge.Api.Notifications;

public interface INotificationAuditRepository
{
    Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification?> GetNotificationAsync(Guid organisationId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetNotificationsAsync(Guid organisationId, Guid recipientUserId, CancellationToken cancellationToken = default);
    Task AddAuditRecordAsync(AuditRecord auditRecord, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditRecord>> GetAuditRecordsAsync(Guid organisationId, string? entityType, Guid? entityId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}