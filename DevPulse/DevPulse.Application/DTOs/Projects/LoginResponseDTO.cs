namespace DevPulse.Application.DTOs.Projects;

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresIn { get; set; }
    public UserDetailDto? User { get; set; }
}