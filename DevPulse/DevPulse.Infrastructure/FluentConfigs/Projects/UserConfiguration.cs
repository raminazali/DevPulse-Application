using DevPulse.Core.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevPulse.Infrastructure.FluentConfigs.Projects;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FullName)
            .HasMaxLength(200);

        builder.Property(x => x.IsAdmin)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired();


        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.IsAdmin);

        builder.HasMany(x => x.Projects)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Email = "admin@devpulse.com",
            PasswordHash = "AQAAAAIAAYagAAAAEDn9DKOBINoN/v+btNrapwv+J5XjHYatXm9gi0fNV+wfcgVHqavjwp1fcB1sVBRAoA==",
            FullName = "مدیر سیستم",
            IsAdmin = true,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
