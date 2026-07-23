using Microsoft.EntityFrameworkCore;

namespace DevPulse.Infrastructure.Context;

public class DevPulseDbContext : DbContext
{
    public DevPulseDbContext(DbContextOptions<DevPulseDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevPulseDbContext).Assembly);
    }
}
