using System;
using System.Collections.Generic;
using System.Text;

namespace DevPulse.Application.DTOs.Projects;

public class ProjectListItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ApiKey { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int ErrorCount { get; set; }
}
