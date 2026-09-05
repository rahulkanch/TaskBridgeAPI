using Microsoft.AspNetCore.Mvc;

namespace TaskBridge.Api.Projects;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(IProjectService projectService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Project>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var project = await projectService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetByTeam), new { teamId = project.TeamId }, project);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPatch("{projectId:guid}/status")]
    public async Task<ActionResult<Project>> UpdateStatus(
        Guid projectId,
        UpdateProjectStatusRequest request,
        CancellationToken cancellationToken)
    {
        var project = await projectService.UpdateStatusAsync(projectId, request.Status, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<ActionResult<IReadOnlyList<Project>>> GetByTeam(
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