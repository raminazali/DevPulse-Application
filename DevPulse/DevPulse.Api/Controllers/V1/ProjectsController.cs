using DevPulse.Api.Extentions;
using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevPulse.Api.Controllers.V1;

[ApiController]
[Authorize(Roles = "Admin,User")]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _projectService.GetByIdAsync(id, User.GetUserIdAsGuid(), User.IsAdmin());

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserProjects(
        [FromQuery] PaginationRequest request)
    {
        var result = await _projectService.GetUserProjectsAsync(User.GetUserIdAsGuid(), User.IsAdmin(), request);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        var result = await _projectService.CreateAsync(request.UserId, request.Name);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            null);
    }

    [HttpPut()]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(UpdateProjectRequest request)
    {
        await _projectService.UpdateAsync(request, User.GetUserIdAsGuid(), User.IsAdmin());
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectService.DeleteAsync(id, User.GetUserIdAsGuid(), User.IsAdmin());

        return NoContent();
    }
}