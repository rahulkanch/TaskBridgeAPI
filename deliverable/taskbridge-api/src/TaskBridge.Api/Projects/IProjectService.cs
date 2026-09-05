namespace TaskBridge.Api.Projects;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<ProjectResponse?> UpdateStatusAsync(Guid projectId, ProjectStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectResponse>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid projectId, CancellationToken cancellationToken = default);
}