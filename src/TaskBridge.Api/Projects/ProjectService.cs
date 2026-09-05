using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Common.Persistence;
using TaskBridge.Api.Common.Security;
using TaskBridge.Api.Notifications;

namespace TaskBridge.Api.Projects;

public sealed class ProjectService(
    TaskBridgeDbContext dbContext,
    ICurrentUserContext currentUser,
    INotificationAuditService notificationAuditService) : IProjectService
{
    public async Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.TeamId == Guid.Empty)
        {
            throw new ArgumentException("TeamId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        var now = DateTimeOffset.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            TeamId = request.TeamId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        await notificationAuditService.RecordProjectChangeAsync(
            new ProjectChange(project.Id, "Created", null, project, null, null, null, null),
            cancellationToken);
        return ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateStatusAsync(
        Guid projectId,
        ProjectStatus status,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .SingleOrDefaultAsync(
                item => item.Id == projectId && item.OrganisationId == currentUser.OrganisationId,
                cancellationToken);

        if (project is null)
        {
            return null;
        }

        var previousState = new Project
        {
            Id = project.Id,
            OrganisationId = project.OrganisationId,
            TeamId = project.TeamId,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
        project.Status = status;
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        await notificationAuditService.RecordProjectChangeAsync(
            new ProjectChange(project.Id, "StatusChanged", previousState, project, null, null, null, null),
            cancellationToken);
        return ToResponse(project);
    }

    public async Task<IReadOnlyList<ProjectResponse>> GetByTeamAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(project =>
                project.TeamId == teamId && project.OrganisationId == currentUser.OrganisationId)
            .OrderByDescending(project => project.CreatedAt)
            .ToListAsync(cancellationToken);

        return projects.Select(ToResponse).ToList();
    }

    public async Task<bool> DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .SingleOrDefaultAsync(
                item => item.Id == projectId && item.OrganisationId == currentUser.OrganisationId,
                cancellationToken);

        if (project is null)
        {
            return false;
        }

        var previousState = new Project
        {
            Id = project.Id,
            OrganisationId = project.OrganisationId,
            TeamId = project.TeamId,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        await notificationAuditService.RecordProjectChangeAsync(
            new ProjectChange(projectId, "Deleted", previousState, null, null, null, null, null),
            cancellationToken);
        return true;
    }

    private static ProjectResponse ToResponse(Project project) => new(
        project.Id,
        project.TeamId,
        project.Name,
        project.Description,
        project.Status,
        project.CreatedAt,
        project.UpdatedAt);
}