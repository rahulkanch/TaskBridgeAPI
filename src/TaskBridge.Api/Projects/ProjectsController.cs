using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TaskBridge.Api.Projects;

[ApiController]
[Authorize]
[Route("api/projects")]
public sealed class ProjectsController(IProjectService projectService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await projectService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByTeam), new { teamId = project.TeamId }, project);
    }

    [HttpPatch("{projectId:guid}/status")]
    public async Task<ActionResult<ProjectResponse>> UpdateStatus(
        Guid projectId,
        UpdateProjectStatusRequest request,
        CancellationToken cancellationToken)
    {
        var project = await projectService.UpdateStatusAsync(projectId, request.Status, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> GetByTeam(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        return Ok(await projectService.GetByTeamAsync(teamId, cancellationToken));
    }

    [HttpDelete("{projectId:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, CancellationToken cancellationToken)
    {
        return await projectService.DeleteAsync(projectId, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}