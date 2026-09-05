namespace TaskBridge.Api.Projects;

public interface IProjectService
{
    Task<Project> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<Project?> UpdateStatusAsync(Guid projectId, ProjectStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid projectId, CancellationToken cancellationToken = default);
}