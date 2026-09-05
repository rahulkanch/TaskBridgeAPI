namespace TaskBridge.Api.Notifications;

public sealed class Notification
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid RecipientUserId { get; set; }
    public Guid? ProjectId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
}