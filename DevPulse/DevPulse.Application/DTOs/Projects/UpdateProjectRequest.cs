namespace DevPulse.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
