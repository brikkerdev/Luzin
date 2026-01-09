using Microsoft.AspNetCore.Authentication;

public sealed class ApiKeyOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ApiKey";

    public string HeaderName { get; set; } = "X-Api-Key";

    public string ConfigurationSectionName { get; set; } = "ApiKeys";
}