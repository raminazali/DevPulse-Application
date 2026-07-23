namespace DevPulse.Application.DTOs.Projects;

public class CreateUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}
