using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Common.Persistence;

namespace TaskBridge.Api.Notifications;

public sealed class NotificationAuditRepository(TaskBridgeDbContext dbContext) : INotificationAuditRepository
{
    public Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        dbContext.Notifications.Add(notification);
        return Task.CompletedTask;
    }

    public Task<Notification?> GetNotificationAsync(
        Guid organisationId,
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Notifications.SingleOrDefaultAsync(
            notification => notification.Id == notificationId
                && notification.OrganisationId == organisationId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(
        Guid organisationId,
        Guid recipientUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.OrganisationId == organisationId
                && notification.RecipientUserId == recipientUserId)
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task AddAuditRecordAsync(AuditRecord auditRecord, CancellationToken cancellationToken = default)
    {
        dbContext.AuditRecords.Add(auditRecord);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<AuditRecord>> GetAuditRecordsAsync(
        Guid organisationId,
        string? entityType,
        Guid? entityId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.AuditRecords
            .AsNoTracking()
            .Where(record => record.OrganisationId == organisationId
                && (entityType == null || record.EntityType == entityType)
                && (entityId == null || record.EntityId == entityId))
            .OrderByDescending(record => record.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}