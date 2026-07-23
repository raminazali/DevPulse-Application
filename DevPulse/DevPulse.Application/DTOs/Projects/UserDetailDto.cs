namespace DevPulse.Application.DTOs.Projects;

public class UserDetailDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ProjectListItemDto> Projects { get; set; } = new List<ProjectListItemDto>();
}