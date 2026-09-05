namespace TaskBridge.Api.Notifications;

public sealed record CreateNotificationRequest(
    Guid RecipientUserId,
    Guid? ProjectId,
    NotificationType Type,
    string Title,
    string Body);

public sealed record NotificationResponse(
    Guid Id,
    Guid? ProjectId,
    NotificationType Type,
    string Title,
    string Body,
    NotificationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ReadAt);

public sealed record AuditRecordResponse(
    Guid Id,
    Guid ActorUserId,
    string EntityType,
    Guid EntityId,
    string Action,
    string? PreviousStateJson,
    string? NewStateJson,
    DateTimeOffset OccurredAt);

public sealed record ProjectChange(
    Guid ProjectId,
    string Action,
    object? PreviousState,
    object? NewState,
    Guid? NotifyUserId,
    NotificationType? NotificationType,
    string? NotificationTitle,
    string? NotificationBody);