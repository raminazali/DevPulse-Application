using DevPulse.Api.Authentication;
using DevPulse.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace DevPulse.Api.Extentions;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the "ApiKey" authentication scheme and its validator.
    /// Call this alongside your existing AddAuthentication(...).AddJwtBearer(...)
    /// setup — it adds a second scheme, it doesn't replace your default one.
    /// </summary>
    public static AuthenticationBuilder AddApiKeyScheme(this AuthenticationBuilder builder)
    {
        builder.Services.AddScoped<IApiKeyValidator, ApiKeyValidator>();

        builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationDefaults.SchemeName,
            options => { });

        return builder;
    }
}
