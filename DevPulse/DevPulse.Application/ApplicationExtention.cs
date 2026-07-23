using DevPulse.Application.Services.Dashboard;
using DevPulse.Application.Services.Dashboard.Interfaces;
using DevPulse.Application.Services.Errors;
using DevPulse.Application.Services.Errors.Inetfaces;
using DevPulse.Application.Services.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Services.Users;
using DevPulse.Core.Entities.Projects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DevPulse.Infrastructure;

public static class ApplicationExtention
{

    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Application Services
        services.AddScoped<IProjectService , ProjectService>();
        services.AddScoped<IUserService , UserService>();
        services.AddScoped<IAuthService , AuthService>();
        services.AddScoped<IErrorService, ErrorService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        // Add logging for services
        services.AddLogging();
    }
}