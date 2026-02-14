using FluentAssertions;
using SyncUp.ClickUpApi.Tests.Fixtures;
using Xunit;

namespace SyncUp.ClickUpApi.Tests.Services;

public class WorkspaceServiceTests
{
    [Fact]
    public async Task GetWorkspacesAsync_Returns_Teams_List()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            teams = new[]
            {
                new { id = "ws_1", name = "Workspace One", color = "#ff0000" },
                new { id = "ws_2", name = "Workspace Two", color = "#00ff00" }
            }
        });

        var workspaces = await client.GetWorkspacesAsync();

        workspaces.Should().HaveCount(2);
        workspaces[0].Id.Should().Be("ws_1");
        workspaces[0].Name.Should().Be("Workspace One");
        workspaces[1].Id.Should().Be("ws_2");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/team");
        handler.LastRequest.Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task GetWorkspacesAsync_Returns_Empty_List_When_No_Workspaces()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { teams = Array.Empty<object>() });

        var workspaces = await client.GetWorkspacesAsync();

        workspaces.Should().BeEmpty();
    }
}
