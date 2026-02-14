using Microsoft.Extensions.Options;
using NSubstitute;
using SyncUp.ClickUpApi.Services;

namespace SyncUp.ClickUpApi.Tests.Fixtures;

/// <summary>
/// Convenience factory for creating a <see cref="ClickUpClient"/> wired to a <see cref="MockHttpMessageHandler"/>.
/// </summary>
public static class ClickUpClientFixture
{
    public const string DefaultBaseUrl = "https://api.clickup.com/api";

    public static (ClickUpClient Client, MockHttpMessageHandler Handler) Create(
        ClickUpOptions? options = null)
    {
        var opts = options ?? new ClickUpOptions
        {
            BaseUrl = DefaultBaseUrl,
            PersonalApiToken = "pk_test_token_123"
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<ClickUpOptions>>();
        optionsMonitor.CurrentValue.Returns(opts);

        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(opts.BaseUrl)
        };

        var client = new ClickUpClient(httpClient, optionsMonitor);
        return (client, handler);
    }
}
