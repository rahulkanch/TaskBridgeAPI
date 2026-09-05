namespace TaskBridge.Api.Notifications;

public sealed class AuditRecord
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid ActorUserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? PreviousStateJson { get; set; }
    public string? NewStateJson { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}