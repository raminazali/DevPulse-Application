namespace DevPulse.Application.DTOs.Projects;

public class ProjectDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ApiKey { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public string UserEmail { get; set; }
}