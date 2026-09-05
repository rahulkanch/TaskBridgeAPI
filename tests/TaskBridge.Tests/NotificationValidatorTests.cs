using FluentAssertions;
using TaskBridge.Api.Notifications;

namespace TaskBridge.Tests;

public sealed class NotificationValidatorTests
{
    [Fact]
    public async Task Create_notification_rejects_empty_or_oversized_content()
    {
        var validator = new CreateNotificationRequestValidator();
        var request = new CreateNotificationRequest(
            Guid.Empty,
            null,
            (NotificationType)999,
            string.Empty,
            new string('x', 4001));

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName)
            .Should().Contain(new[] { "RecipientUserId", "Type", "Title", "Body" });
    }
}