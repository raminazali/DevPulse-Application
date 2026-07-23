namespace DevPulse.Application.DTOs.Projects;

public class CreateProjectRequest
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;
}
