using DevPulse.Core.Entities.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevPulse.Infrastructure.FluentConfigs.Errors;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StackTrace)
            .IsRequired();

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ExceptionType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Method)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.RequestBody);

        builder.Property(x => x.QueryString)
            .HasMaxLength(2000);

        builder.Property(x => x.UserId)
            .HasMaxLength(450);

        builder.Property(x => x.Browser)
            .HasMaxLength(500);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.ProjectId);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => new
        {
            x.ProjectId,
            x.CreatedAt
        });

        builder.HasIndex(x => x.ExceptionType);

        builder.HasIndex(x => x.Url);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.ErrorLogs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}