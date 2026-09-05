using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Common.Persistence;

namespace TaskBridge.Api.Projects;

public sealed class ProjectService(TaskBridgeDbContext dbContext) : IProjectService
{
    public async Task<Project> CreateAsync(
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
            TeamId = request.TeamId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<Project?> UpdateStatusAsync(
        Guid projectId,
        ProjectStatus status,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .SingleOrDefaultAsync(item => item.Id == projectId, cancellationToken);

        if (project is null)
        {
            return null;
        }

        project.Status = status;
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<IReadOnlyList<Project>> GetByTeamAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.TeamId == teamId)
            .OrderByDescending(project => project.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .SingleOrDefaultAsync(item => item.Id == projectId, cancellationToken);

        if (project is null)
        {
            return false;
        }

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}