using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DevPulse.Api.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService, IUserService userService) => _authService = authService;

    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
    {
        return Ok(await _authService.LoginAsync(request));
    }
}