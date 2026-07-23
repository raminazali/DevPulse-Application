using DevPulse.Core.Interfaces;
using DevPulse.Infrastructure.Context;
using DevPulse.Infrastructure.Repository;
using DevPulse.Infrastructure.Repository.Contracts;
using DevPulse.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevPulse.Infrastructure;

public static class InfrastructureExtention
{

    public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Add S3 Storage Service
        services.AddS3StorageService(configuration);
        services.AddScoped<ICacheService, CacheService>();
        // Add Context
        services.AddDevPulseDbContext(configuration);

    }

    /// <summary>
    /// Registers S3 storage service with configuration and AWS SDK client.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    private static void AddS3StorageService(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind S3Settings from configuration
        //services.Configure<S3Settings>(configuration.GetSection("S3Settings"));

        // Register AWS S3 client
        //services.AddSingleton<IAmazonS3>(sp =>
        //{
        //    var s3Settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<S3Settings>>();
        //    var s3Config = new Amazon.S3.AmazonS3Config 
        //    { 
        //        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(s3Settings.Value.Region)
        //    };
        //    return new Amazon.S3.AmazonS3Client(s3Config);
        //});
            
        // Register S3StorageService
        //services.AddScoped<IS3StorageService, S3StorageService>();
    }

    private static void AddDevPulseDbContext(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddDbContext<DevPulseDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DevPulse"));
        });
    }
}
