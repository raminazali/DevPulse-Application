using DevPulse.Core.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevPulse.Infrastructure.FluentConfigs.Projects;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ApiKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.ApiKey)
            .IsUnique();

        builder.HasIndex(x => x.IsActive);
    }
}