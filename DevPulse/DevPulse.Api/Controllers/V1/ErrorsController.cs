using DevPulse.Api.Extentions;
using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Errors;
using DevPulse.Application.Services.Errors.Inetfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevPulse.Api.Controllers.V1;

[Route("api/[controller]")]
[Authorize(Roles = "Admin,User")]
[ApiController]
public class ErrorsController : ControllerBase
{
    private readonly IErrorService _errorService;

    public ErrorsController(IErrorService errorService) => _errorService = errorService;

    /// <summary>
    /// Retrieves a single ErrorLog by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ErrorDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ErrorDetailDto>> GetErrorById(Guid id, CancellationToken cancellationToken)
    {
        var errorDetail = await _errorService.GetErrorByIdAsync(id, cancellationToken);
        return Ok(errorDetail);
    }

    /// <summary>
    /// Returns a paged, filterable list of ErrorLog records.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ErrorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ErrorListItemDto>>> GetErrors(
        [FromQuery] ErrorFilterOptions filter,
        CancellationToken cancellationToken)
    {

        return Ok(await _errorService.GetErrorsAsync(filter, User.GetUserIdAsGuid() , User.IsAdmin(), cancellationToken));
    }
}