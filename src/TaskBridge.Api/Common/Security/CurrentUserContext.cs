using System.Security.Claims;

namespace TaskBridge.Api.Common.Security;

public sealed class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public Guid UserId => GetRequiredGuidClaim(ClaimTypes.NameIdentifier, "sub");

    public Guid OrganisationId => GetRequiredGuidClaim("organisation_id");

    private Guid GetRequiredGuidClaim(params string[] claimTypes)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var claim = claimTypes
            .Select(type => user?.FindFirst(type)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        return Guid.TryParse(claim, out var value) && value != Guid.Empty
            ? value
            : throw new UnauthorizedAccessException("Required identity claim is missing.");
    }
}