using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyOptions>
{
    private readonly IConfiguration _config;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration config) : base(options, logger, encoder)
    {
        _config = config;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var values))
            return Task.FromResult(AuthenticateResult.NoResult());

        var providedKey = values.ToString();
        if (string.IsNullOrWhiteSpace(providedKey))
            return Task.FromResult(AuthenticateResult.Fail("API key is missing."));

        var allowed = _config.GetSection(Options.ConfigurationSectionName).GetChildren()
            .ToDictionary(x => x.Key, x => x.Value ?? string.Empty, StringComparer.Ordinal);

        if (!allowed.TryGetValue(providedKey, out var owner))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, owner),
            new(ClaimTypes.Name, owner),
            new("auth_type", "api_key"),
        };

        var identity = new ClaimsIdentity(claims, ApiKeyOptions.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ApiKeyOptions.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}