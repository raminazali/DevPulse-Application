using DevPulse.Api.Authentication;
using DevPulse.Application.DTOs.Errors;
using DevPulse.Application.Services.Errors.Inetfaces;
using DevPulse.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DevPulse.Api.Controllers.V1.External;

/// <summary>
/// Ingestion endpoint for external projects to report their own exceptions.
/// Authenticated via API key (not the dashboard's JWT scheme) — see
/// ApiKeyAuthenticationHandler.
/// </summary>
[Route("api/external/[controller]")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.SchemeName)]
[ApiController]
public class ErrorsController : ControllerBase
{
    private readonly IErrorService _errorService;
    public ErrorsController(IErrorService errorService) => _errorService = errorService;

    /// <summary>
    /// Logs a single exception as one standalone ErrorLog row. multipart/form-data
    /// is used (rather than JSON) because CreateErrorRequest carries an optional
    /// IFormFile screenshot, which cannot bind from a JSON body.
    ///
    /// ProjectId is taken from the authenticated API key's claim, not from the
    /// request body — a caller's own ProjectId is never trusted, otherwise any
    /// external project could write errors into someone else's project.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("ErrorCreatePolicy")]
    [ProducesResponseType(typeof(ErrorDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ErrorDetailDto>> CreateError([FromBody] CreateErrorRequest request, CancellationToken cancellationToken)
    {
        request.ProjectId = GetProjectId(User);

        var errorDetail = await _errorService.CreateErrorAsync(request, cancellationToken);

        return Created($"/api/errors/{errorDetail.Id}", errorDetail);
    }

    private Guid GetProjectId(ClaimsPrincipal User)
    {
        var projectIdClaim = User.FindFirstValue("ProjectId");
        if (!Guid.TryParse(projectIdClaim, out var projectId))
            throw new UnauthorizedException("شناسه پروژه خالی می باشد.", "AUTH_PROJECT_NOT_FOUND");
        return projectId;
    }
}