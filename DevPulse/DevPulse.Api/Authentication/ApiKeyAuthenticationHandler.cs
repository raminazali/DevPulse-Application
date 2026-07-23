using DevPulse.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DevPulse.Api.Authentication;

/// <summary>
/// Authenticates external callers via an API key header. On success, the
/// resulting principal carries a "ProjectId" claim identifying which project
/// the key belongs to — controllers should trust that claim, not any
/// project id the caller puts in the request body.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyValidator apiKeyValidator)
        : base(options, logger, encoder)
    {
        _apiKeyValidator = apiKeyValidator ?? throw new ArgumentNullException(nameof(apiKeyValidator));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValues))
            return AuthenticateResult.Fail($"Missing '{Options.HeaderName}' header.");

        var apiKey = headerValues.ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
            return AuthenticateResult.Fail($"'{Options.HeaderName}' header is empty.");

        var principal = await _apiKeyValidator.ValidateAsync(apiKey, Context.RequestAborted);
        if (principal == null)
            return AuthenticateResult.Fail("Invalid API key.");

        var claims = new[]
        {
            new Claim("ProjectId", principal.ProjectId.ToString()),
            new Claim("ProjectName", principal.ProjectName),
            new Claim(ClaimTypes.AuthenticationMethod, ApiKeyAuthenticationDefaults.SchemeName)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}