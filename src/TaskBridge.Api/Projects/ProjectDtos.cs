namespace TaskBridge.Api.Projects;

public sealed record CreateProjectRequest(
    Guid TeamId,
    string Name,
    string? Description,
    ProjectStatus Status = ProjectStatus.Planning);

public sealed record UpdateProjectStatusRequest(ProjectStatus Status);

public sealed record ProjectResponse(
    Guid Id,
    Guid TeamId,
    string Name,
    string? Description,
    ProjectStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);