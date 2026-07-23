namespace DevPulse.Application.DTOs.Projects;

public class UserListItemDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int ProjectCount { get; set; }
}