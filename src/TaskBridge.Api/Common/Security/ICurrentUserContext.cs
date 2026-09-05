namespace TaskBridge.Api.Common.Security;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    Guid OrganisationId { get; }
}