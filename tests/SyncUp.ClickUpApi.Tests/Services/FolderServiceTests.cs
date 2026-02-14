using System.Net;
using FluentAssertions;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Tests.Fixtures;
using Xunit;

namespace SyncUp.ClickUpApi.Tests.Services;

public class FolderServiceTests
{
    [Fact]
    public async Task GetFoldersAsync_Returns_Folders_For_Space()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            folders = new[]
            {
                new { id = "f_1", name = "Sprint 1", orderindex = 0 },
                new { id = "f_2", name = "Sprint 2", orderindex = 1 }
            }
        });

        var folders = await client.GetFoldersAsync("sp_1");

        folders.Should().HaveCount(2);
        folders[0].Name.Should().Be("Sprint 1");
        handler.LastRequest!.RequestUri!.PathAndQuery
            .Should().StartWith("/api/v2/space/sp_1/folder");
    }

    [Fact]
    public async Task GetFolderAsync_Returns_Single_Folder()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "f_1", name = "My Folder", task_count = "5" });

        var folder = await client.GetFolderAsync("f_1");

        folder.Id.Should().Be("f_1");
        folder.TaskCount.Should().Be("5");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/folder/f_1");
    }

    [Fact]
    public async Task CreateFolderAsync_Posts_And_Returns_Folder()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "f_new", name = "New Folder" });

        var folder = await client.CreateFolderAsync("sp_1", new CreateFolderRequest { Name = "New Folder" });

        folder.Name.Should().Be("New Folder");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/space/sp_1/folder");
    }

    [Fact]
    public async Task UpdateFolderAsync_Puts_And_Returns_Folder()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "f_1", name = "Updated" });

        var folder = await client.UpdateFolderAsync("f_1", new UpdateFolderRequest { Name = "Updated" });

        folder.Name.Should().Be("Updated");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
    }

    [Fact]
    public async Task DeleteFolderAsync_Sends_Delete()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueResponse(HttpStatusCode.OK);

        await client.DeleteFolderAsync("f_1");

        handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/folder/f_1");
    }
}
