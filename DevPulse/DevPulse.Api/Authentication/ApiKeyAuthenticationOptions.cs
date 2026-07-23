using Microsoft.AspNetCore.Authentication;

namespace DevPulse.Api.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// The HTTP header external callers put their key in.
    /// </summary>
    public string HeaderName { get; set; } = "X-Api-Key";
}

public static class ApiKeyAuthenticationDefaults
{
    public const string SchemeName = "ApiKey";
}
