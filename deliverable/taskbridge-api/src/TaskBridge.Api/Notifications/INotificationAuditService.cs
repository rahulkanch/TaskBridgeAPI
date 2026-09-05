namespace TaskBridge.Api.Notifications;

public interface INotificationAuditService
{
    Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationResponse>> GetMyNotificationsAsync(CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditRecordResponse>> GetAuditRecordsAsync(string? entityType, Guid? entityId, CancellationToken cancellationToken = default);
    Task RecordProjectChangeAsync(ProjectChange change, CancellationToken cancellationToken = default);
}