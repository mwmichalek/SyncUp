using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace SyncUp.ClickUpApi.Auth;

/// <summary>
/// DelegatingHandler that injects the ClickUp Authorization header on every request.
/// Supports both personal API tokens and OAuth access tokens.
/// </summary>
public sealed class ClickUpAuthHandler : DelegatingHandler
{
    private readonly IOptionsMonitor<ClickUpOptions> _options;

    public ClickUpAuthHandler(IOptionsMonitor<ClickUpOptions> options)
    {
        _options = options;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _options.CurrentValue.GetEffectiveToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
