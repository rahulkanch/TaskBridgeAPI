using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Common.Persistence;
using TaskBridge.Api.Common.Security;
using TaskBridge.Api.Notifications;
using TaskBridge.Api.Projects;

namespace TaskBridge.Tests;

public sealed class ProjectServiceTests
{
    [Fact]
    public async Task CreateAsync_uses_claimed_organisation_and_records_audit()
    {
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        var auditService = CreateAuditService(dbContext, organisationId, userId);
        var service = new ProjectService(dbContext, new TestUserContext(userId, organisationId), auditService);

        var result = await service.CreateAsync(new CreateProjectRequest(
            Guid.NewGuid(), "Bridge", "A project", ProjectStatus.Planning));

        var project = await dbContext.Projects.SingleAsync();
        project.OrganisationId.Should().Be(organisationId);
        result.Name.Should().Be("Bridge");
        (await dbContext.AuditRecords.SingleAsync()).Action.Should().Be("Created");
    }

    [Fact]
    public async Task Operations_do_not_cross_organisation_boundaries()
    {
        var organisationA = Guid.NewGuid();
        var organisationB = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            OrganisationId = organisationA,
            TeamId = teamId,
            Name = "Private project",
            Status = ProjectStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();

        var service = new ProjectService(
            dbContext,
            new TestUserContext(Guid.NewGuid(), organisationB),
            CreateAuditService(dbContext, organisationB, Guid.NewGuid()));

        (await service.GetByTeamAsync(teamId)).Should().BeEmpty();
        (await service.UpdateStatusAsync(project.Id, ProjectStatus.Completed)).Should().BeNull();
        (await service.DeleteAsync(project.Id)).Should().BeFalse();
        (await dbContext.Projects.FindAsync(project.Id)).Should().NotBeNull();
    }

    private static TaskBridgeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskBridgeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TaskBridgeDbContext(options);
    }

    private static INotificationAuditService CreateAuditService(
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