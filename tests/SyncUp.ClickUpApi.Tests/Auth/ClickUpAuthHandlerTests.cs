using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SyncUp.ClickUpApi.Auth;
using Xunit;

namespace SyncUp.ClickUpApi.Tests.Auth;

public class ClickUpAuthHandlerTests
{
    private static (ClickUpAuthHandler Handler, HttpMessageInvoker Invoker) CreateHandler(ClickUpOptions options)
    {
        var optionsMonitor = Substitute.For<IOptionsMonitor<ClickUpOptions>>();
        optionsMonitor.CurrentValue.Returns(options);

        var authHandler = new ClickUpAuthHandler(optionsMonitor)
        {
            InnerHandler = new StubInnerHandler()
        };

        var invoker = new HttpMessageInvoker(authHandler);
        return (authHandler, invoker);
    }

    [Fact]
    public async Task Sets_Authorization_Header_With_PersonalApiToken()
    {
        var (_, invoker) = CreateHandler(new ClickUpOptions
        {
            PersonalApiToken = "pk_12345"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        var response = await invoker.SendAsync(request, CancellationToken.None);

        request.Headers.Authorization.Should().NotBeNull();
        request.Headers.Authorization!.Scheme.Should().Be("pk_12345");
    }

    [Fact]
    public async Task Sets_Authorization_Header_With_OAuthAccessToken()
    {
        var (_, invoker) = CreateHandler(new ClickUpOptions
        {
            OAuthAccessToken = "oauth_token_abc"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        await invoker.SendAsync(request, CancellationToken.None);

        request.Headers.Authorization!.Scheme.Should().Be("oauth_token_abc");
    }

    [Fact]
    public async Task PersonalApiToken_Takes_Precedence_Over_OAuth()
    {
        var (_, invoker) = CreateHandler(new ClickUpOptions
        {
            PersonalApiToken = "pk_wins",
            OAuthAccessToken = "oauth_loses"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        await invoker.SendAsync(request, CancellationToken.None);

        request.Headers.Authorization!.Scheme.Should().Be("pk_wins");
    }

    [Fact]
    public async Task No_Token_Configured_Does_Not_Set_Header()
    {
        var (_, invoker) = CreateHandler(new ClickUpOptions());

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        await invoker.SendAsync(request, CancellationToken.None);

        request.Headers.Authorization.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Whitespace_Token_Does_Not_Set_Header(string token)
    {
        var (_, invoker) = CreateHandler(new ClickUpOptions
        {
            PersonalApiToken = token
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        await invoker.SendAsync(request, CancellationToken.None);

        request.Headers.Authorization.Should().BeNull();
    }

    /// <summary>Stub inner handler that returns 200 OK so the pipeline completes.</summary>
    private sealed class StubInnerHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}
