using System.Net;
using FluentAssertions;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Tests.Fixtures;

namespace SyncUp.ClickUpApi.Tests.Services;

public class SpaceServiceTests
{
    [Fact]
    public async Task GetSpacesAsync_Sends_Correct_Url_With_Archived_Param()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            spaces = new[]
            {
                new { id = "sp_1", name = "Dev Space", @private = false }
            }
        });

        var spaces = await client.GetSpacesAsync("ws_1", archived: true);

        spaces.Should().HaveCount(1);
        spaces[0].Id.Should().Be("sp_1");
        spaces[0].Name.Should().Be("Dev Space");
        handler.LastRequest!.RequestUri!.PathAndQuery
            .Should().Be("/api/v2/team/ws_1/space?archived=true");
    }

    [Fact]
    public async Task GetSpacesAsync_Defaults_Archived_To_False()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { spaces = Array.Empty<object>() });

        await client.GetSpacesAsync("ws_1");

        handler.LastRequest!.RequestUri!.PathAndQuery
            .Should().Contain("archived=false");
    }

    [Fact]
    public async Task GetSpaceAsync_Sends_Correct_Url()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "sp_99", name = "My Space" });

        var space = await client.GetSpaceAsync("sp_99");

        space.Id.Should().Be("sp_99");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/space/sp_99");
    }

    [Fact]
    public async Task CreateSpaceAsync_Posts_Body_And_Returns_Space()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "sp_new", name = "New Space" });

        var space = await client.CreateSpaceAsync("ws_1", new CreateSpaceRequest { Name = "New Space" });

        space.Name.Should().Be("New Space");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/team/ws_1/space");

        var body = await handler.GetLastRequestBodyAsync();
        body.Should().Contain("New Space");
    }

    [Fact]
    public async Task UpdateSpaceAsync_Puts_Body()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "sp_1", name = "Renamed" });

        var space = await client.UpdateSpaceAsync("sp_1", new UpdateSpaceRequest { Name = "Renamed" });

        space.Name.Should().Be("Renamed");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/space/sp_1");
    }

    [Fact]
    public async Task DeleteSpaceAsync_Sends_Delete()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueResponse(HttpStatusCode.OK);

        await client.DeleteSpaceAsync("sp_1");

        handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/space/sp_1");
    }

    [Fact]
    public async Task DeleteSpaceAsync_Throws_On_NotFound()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueError(HttpStatusCode.NotFound);

        var act = () => client.DeleteSpaceAsync("nonexistent");

        await act.Should().ThrowAsync<ClickUpApiException>()
            .Where(ex => ex.StatusCode == HttpStatusCode.NotFound);
    }
}
