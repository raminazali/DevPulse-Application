using DevPulse.Application.DTOs.Projects;

namespace DevPulse.Application.Services.Projects.Inetfaces;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
}