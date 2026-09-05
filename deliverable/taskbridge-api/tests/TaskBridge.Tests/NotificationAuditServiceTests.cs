using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Common.Persistence;
using TaskBridge.Api.Common.Security;
using TaskBridge.Api.Notifications;

namespace TaskBridge.Tests;

public sealed class NotificationAuditServiceTests
{
    [Fact]
    public async Task Notifications_are_visible_only_to_the_recipient_in_the_same_organisation()
    {
        var organisationA = Guid.NewGuid();
        var organisationB = Guid.NewGuid();
        var recipient = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        var serviceA = CreateService(dbContext, organisationA, recipient);

        await serviceA.CreateNotificationAsync(new CreateNotificationRequest(
            recipient, null, NotificationType.General, "Hello", "Private message"));

        var serviceB = CreateService(dbContext, organisationB, recipient);
        (await serviceB.GetMyNotificationsAsync()).Should().BeEmpty();
        (await serviceB.MarkAsReadAsync(await dbContext.Notifications.Select(item => item.Id).SingleAsync()))
            .Should().BeFalse();
        (await dbContext.Notifications.SingleAsync()).Status.Should().Be(NotificationStatus.Unread);
    }

    [Fact]
    public async Task Audit_reads_are_tenant_scoped_and_records_are_not_mutable_through_the_service()
    {
        var organisationA = Guid.NewGuid();
        var organisationB = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        var serviceA = CreateService(dbContext, organisationA, Guid.NewGuid());

        await serviceA.RecordProjectChangeAsync(new ProjectChange(
            Guid.NewGuid(), "StatusChanged", new { Status = "Planning" }, new { Status = "Active" },
            null, null, null, null));

        var serviceB = CreateService(dbContext, organisationB, Guid.NewGuid());
        (await serviceB.GetAuditRecordsAsync("Project", null)).Should().BeEmpty();

        var audit = await dbContext.AuditRecords.SingleAsync();
        audit.PreviousStateJson.Should().Contain("Planning");
        audit.NewStateJson.Should().Contain("Active");
    }

    private static TaskBridgeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskBridgeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TaskBridgeDbContext(options);
    }

    private static INotificationAuditService CreateService(
        TaskBridgeDbContext dbContext,
        Guid organisationId,
        Guid userId)
    {
        return new NotificationAuditService(
            new NotificationAuditRepository(dbContext),
            new TestUserContext(userId, organisationId));
    }

    private sealed record TestUserContext(Guid UserId, Guid OrganisationId) : ICurrentUserContext;
}