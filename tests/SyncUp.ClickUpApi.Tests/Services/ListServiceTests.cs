using System.Net;
using FluentAssertions;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Tests.Fixtures;

namespace SyncUp.ClickUpApi.Tests.Services;

public class ListServiceTests
{
    [Fact]
    public async Task GetListsAsync_Returns_Lists_In_Folder()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            lists = new[]
            {
                new { id = "lst_1", name = "Backlog", task_count = 12 },
                new { id = "lst_2", name = "In Progress", task_count = 3 }
            }
        });

        var lists = await client.GetListsAsync("f_1");

        lists.Should().HaveCount(2);
        lists[0].Name.Should().Be("Backlog");
        lists[0].TaskCount.Should().Be(12);
        handler.LastRequest!.RequestUri!.PathAndQuery
            .Should().StartWith("/api/v2/folder/f_1/list");
    }

    [Fact]
    public async Task GetFolderlessListsAsync_Uses_Space_Url()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { lists = Array.Empty<object>() });

        await client.GetFolderlessListsAsync("sp_1", archived: true);

        handler.LastRequest!.RequestUri!.PathAndQuery
            .Should().Be("/api/v2/space/sp_1/list?archived=true");
    }

    [Fact]
    public async Task GetListAsync_Returns_Single_List()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "lst_1", name = "My List", content = "Description here" });

        var list = await client.GetListAsync("lst_1");

        list.Id.Should().Be("lst_1");
        list.Content.Should().Be("Description here");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/list/lst_1");
    }

    [Fact]
    public async Task CreateListAsync_Posts_To_Folder_Endpoint()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "lst_new", name = "New List" });

        var list = await client.CreateListAsync("f_1", new CreateListRequest { Name = "New List" });

        list.Name.Should().Be("New List");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/folder/f_1/list");
    }

    [Fact]
    public async Task CreateFolderlessListAsync_Posts_To_Space_Endpoint()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "lst_new", name = "Folderless" });

        var list = await client.CreateFolderlessListAsync("sp_1", new CreateListRequest { Name = "Folderless" });

        list.Name.Should().Be("Folderless");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/space/sp_1/list");
    }

    [Fact]
    public async Task UpdateListAsync_Puts_Body()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "lst_1", name = "Updated" });

        var list = await client.UpdateListAsync("lst_1", new UpdateListRequest { Name = "Updated" });

        list.Name.Should().Be("Updated");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
    }

    [Fact]
    public async Task DeleteListAsync_Sends_Delete()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueResponse(HttpStatusCode.OK);

        await client.DeleteListAsync("lst_1");

        handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/list/lst_1");
    }

    [Fact]
    public async Task GetCustomFieldsAsync_Returns_Fields()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            fields = new[]
            {
                new { id = "cf_1", name = "Priority Label", type = "drop_down" },
                new { id = "cf_2", name = "Story Points", type = "number" }
            }
        });

        var response = await client.GetCustomFieldsAsync("lst_1");

        response.Fields.Should().HaveCount(2);
        response.Fields[0].Name.Should().Be("Priority Label");
        response.Fields[1].Type.Should().Be("number");
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/list/lst_1/field");
    }
}
