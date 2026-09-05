using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskBridge.Api.Notifications;

[ApiController]
[Authorize]
[Route("api/audits")]
public sealed class AuditsController(INotificationAuditService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditRecordResponse>>> Get(
        [FromQuery] string? entityType,
        [FromQuery] Guid? entityId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetAuditRecordsAsync(entityType, entityId, cancellationToken));
    }
}