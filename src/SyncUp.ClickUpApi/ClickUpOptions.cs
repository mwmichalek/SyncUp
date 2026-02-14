namespace SyncUp.ClickUpApi;

/// <summary>
/// Configuration options for the ClickUp API client.
/// </summary>
public sealed class ClickUpOptions
{
    public const string SectionName = "ClickUp";

    /// <summary>
    /// Base URL for the ClickUp API. Defaults to https://api.clickup.com/api.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.clickup.com/api";

    /// <summary>
    /// Personal API token (pk_...). Use this for simple authentication.
    /// Mutually exclusive with OAuth settings for auth header.
    /// </summary>
    public string? PersonalApiToken { get; set; }

    /// <summary>
    /// OAuth 2.0 client ID (for the authorization code flow).
    /// </summary>
    public string? OAuthClientId { get; set; }

    /// <summary>
    /// OAuth 2.0 client secret.
    /// </summary>
    public string? OAuthClientSecret { get; set; }

    /// <summary>
    /// OAuth 2.0 access token (set after completing the OAuth flow).
    /// </summary>
    public string? OAuthAccessToken { get; set; }

    /// <summary>
    /// Optional default workspace/team ID to use when not specified per-request.
    /// </summary>
    public string? DefaultWorkspaceId { get; set; }

    /// <summary>
    /// Returns the effective bearer token to use (personal token takes precedence).
    /// </summary>
    internal string? GetEffectiveToken() =>
        !string.IsNullOrWhiteSpace(PersonalApiToken) ? PersonalApiToken
        : !string.IsNullOrWhiteSpace(OAuthAccessToken) ? OAuthAccessToken
        : null;
}
