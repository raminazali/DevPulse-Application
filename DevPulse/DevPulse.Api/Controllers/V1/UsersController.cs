using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace DevPulse.Api.Controllers.V1;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
    {
        return Ok(await _userService.GetUsersAsync(request));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}