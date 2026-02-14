using System.Net;
using FluentAssertions;
using SyncUp.ClickUpApi.Tests.Fixtures;
using Xunit;

namespace SyncUp.ClickUpApi.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task GetAccessTokenAsync_Posts_To_OAuth_Endpoint_And_Returns_Token()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { access_token = "oauth_token_abc" });

        var result = await client.GetAccessTokenAsync("client_id", "client_secret", "auth_code");

        result.AccessToken.Should().Be("oauth_token_abc");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/oauth/token");

        var body = await handler.GetLastRequestBodyAsync();
        body.Should().Contain("client_id");
        body.Should().Contain("client_secret");
        body.Should().Contain("auth_code");
    }

    [Fact]
    public async Task GetAuthorizedUserAsync_Returns_User_From_Wrapper()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            user = new
            {
                id = 42,
                username = "Mark",
                email = "mark@example.com",
                timezone = "America/New_York"
            }
        });

        var user = await client.GetAuthorizedUserAsync();

        user.Id.Should().Be(42);
        user.Username.Should().Be("Mark");
        user.Email.Should().Be("mark@example.com");
        user.Timezone.Should().Be("America/New_York");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/user");
    }

    [Fact]
    public async Task GetAuthorizedUserAsync_Throws_On_401()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueError(HttpStatusCode.Unauthorized, """{"err":"Token invalid"}""");

        var act = () => client.GetAuthorizedUserAsync();

        var ex = await act.Should().ThrowAsync<ClickUpApiException>();
        ex.Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        ex.Which.ResponseBody.Should().Contain("Token invalid");
    }
}
