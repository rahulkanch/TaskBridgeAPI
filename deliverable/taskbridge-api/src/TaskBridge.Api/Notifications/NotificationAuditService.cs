using System.Text.Json;
using TaskBridge.Api.Common.Security;

namespace TaskBridge.Api.Notifications;

public sealed class NotificationAuditService(
    INotificationAuditRepository repository,
    ICurrentUserContext currentUser) : INotificationAuditService
{
    public async Task<NotificationResponse> CreateNotificationAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            RecipientUserId = request.RecipientUserId,
            ProjectId = request.ProjectId,
            Type = request.Type,
            Title = request.Title.Trim(),
            Body = request.Body.Trim(),
            Status = NotificationStatus.Unread,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await repository.AddNotificationAsync(notification, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ToResponse(notification);
    }

    public async Task<IReadOnlyList<NotificationResponse>> GetMyNotificationsAsync(
        CancellationToken cancellationToken = default)
    {
        var notifications = await repository.GetNotificationsAsync(
            currentUser.OrganisationId,
            currentUser.UserId,
            cancellationToken);

        return notifications.Select(ToResponse).ToList();
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetNotificationAsync(
            currentUser.OrganisationId,
            notificationId,
            cancellationToken);

        if (notification is null || notification.RecipientUserId != currentUser.UserId)
        {
            return false;
        }

        notification.Status = NotificationStatus.Read;
        notification.ReadAt ??= DateTimeOffset.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<AuditRecordResponse>> GetAuditRecordsAsync(
        string? entityType,
        Guid? entityId,
        CancellationToken cancellationToken = default)
    {
        var records = await repository.GetAuditRecordsAsync(
            currentUser.OrganisationId,
            entityType?.Trim(),
            entityId,
            cancellationToken);

        return records.Select(ToResponse).ToList();
    }

    public async Task RecordProjectChangeAsync(
        ProjectChange change,
        CancellationToken cancellationToken = default)
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var auditRecord = new AuditRecord
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ActorUserId = currentUser.UserId,
            EntityType = "Project",
            EntityId = change.ProjectId,
            Action = change.Action,
            PreviousStateJson = Serialize(change.PreviousState),
            NewStateJson = Serialize(change.NewState),
            OccurredAt = occurredAt
        };

        await repository.AddAuditRecordAsync(auditRecord, cancellationToken);

        if (change.NotifyUserId is Guid recipientUserId && change.NotificationType is NotificationType notificationType)
        {
            await repository.AddNotificationAsync(new Notification
            {
                Id = Guid.NewGuid(),
                OrganisationId = currentUser.OrganisationId,
                RecipientUserId = recipientUserId,
                ProjectId = change.ProjectId,
                Type = notificationType,
                Title = change.NotificationTitle ?? "Project updated",
                Body = change.NotificationBody ?? "A project changed.",
                Status = NotificationStatus.Unread,
                CreatedAt = occurredAt
            }, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
    }

    private static string? Serialize(object? value) => value is null
        ? null
        : JsonSerializer.Serialize(value);

    private static NotificationResponse ToResponse(Notification notification) => new(
        notification.Id,
        notification.ProjectId,
        notification.Type,
        notification.Title,
        notification.Body,
        notification.Status,
        notification.CreatedAt,
        notification.ReadAt);

    private static AuditRecordResponse ToResponse(AuditRecord record) => new(
        record.Id,
        record.ActorUserId,
        record.EntityType,
        record.EntityId,
        record.Action,
        record.PreviousStateJson,
        record.NewStateJson,
        record.OccurredAt);
}