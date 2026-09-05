using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskBridge.Api.Notifications;

[ApiController]
[Authorize]
[Route("api/notifications")]
public sealed class NotificationsController(INotificationAuditService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NotificationResponse>> Create(
        CreateNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var notification = await service.CreateNotificationAsync(request, cancellationToken);
        return Ok(notification);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> GetMine(
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetMyNotificationsAsync(cancellationToken));
    }

    [HttpPatch("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        return await service.MarkAsReadAsync(notificationId, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}