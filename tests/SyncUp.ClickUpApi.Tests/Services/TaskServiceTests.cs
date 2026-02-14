using System.Net;
using FluentAssertions;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Tests.Fixtures;
using Xunit;

namespace SyncUp.ClickUpApi.Tests.Services;

public class TaskServiceTests
{
    // ──────────────────────────────────────────
    //  GetTasks
    // ──────────────────────────────────────────

    [Fact]
    public async Task GetTasksAsync_Returns_Tasks_For_List()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            tasks = new[]
            {
                new { id = "t_1", name = "Task One" },
                new { id = "t_2", name = "Task Two" }
            },
            last_page = true
        });

        var response = await client.GetTasksAsync("lst_1");

        response.Tasks.Should().HaveCount(2);
        response.Tasks[0].Name.Should().Be("Task One");
        response.LastPage.Should().BeTrue();
        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/list/lst_1/task");
    }

    [Fact]
    public async Task GetTasksAsync_With_Query_Appends_Parameters()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { tasks = Array.Empty<object>(), last_page = true });

        var query = new GetTasksQuery
        {
            Page = 2,
            IncludeClosed = true,
            OrderBy = "due_date",
            Reverse = true,
            Subtasks = true,
            Archived = false,
        };

        await client.GetTasksAsync("lst_1", query);

        var uri = handler.LastRequest!.RequestUri!.PathAndQuery;
        uri.Should().Contain("page=2");
        uri.Should().Contain("include_closed=true");
        uri.Should().Contain("order_by=due_date");
        uri.Should().Contain("reverse=true");
        uri.Should().Contain("subtasks=true");
        uri.Should().Contain("archived=false");
    }

    [Fact]
    public async Task GetTasksAsync_Null_Query_Sends_No_QueryString()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { tasks = Array.Empty<object>(), last_page = true });

        await client.GetTasksAsync("lst_1", query: null);

        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/list/lst_1/task");
    }

    // ──────────────────────────────────────────
    //  GetTask
    // ──────────────────────────────────────────

    [Fact]
    public async Task GetTaskAsync_Returns_Task_By_Id()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            id = "abc123",
            name = "My Task",
            description = "A test task",
            status = new { status = "open", color = "#d3d3d3", orderindex = 0, type = "open" },
            url = "https://app.clickup.com/t/abc123"
        });

        var task = await client.GetTaskAsync("abc123");

        task.Id.Should().Be("abc123");
        task.Name.Should().Be("My Task");
        task.Description.Should().Be("A test task");
        task.Status!.Status.Should().Be("open");
        task.Url.Should().Contain("abc123");
    }

    [Fact]
    public async Task GetTaskAsync_With_Subtasks_Flag_Appends_QueryString()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "t_1", name = "Parent" });

        await client.GetTaskAsync("t_1", includeSubtasks: true);

        handler.LastRequest!.RequestUri!.PathAndQuery
            .Should().Be("/api/v2/task/t_1?include_subtasks=true");
    }

    [Fact]
    public async Task GetTaskAsync_Without_Subtasks_Flag_No_QueryString()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "t_1", name = "Simple" });

        await client.GetTaskAsync("t_1");

        handler.LastRequest!.RequestUri!.PathAndQuery.Should().Be("/api/v2/task/t_1");
    }

    // ──────────────────────────────────────────
    //  CreateTask
    // ──────────────────────────────────────────

    [Fact]
    public async Task CreateTaskAsync_Posts_Request_And_Returns_Task()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new
        {
            id = "t_new",
            name = "New Task",
            status = new { status = "to do" }
        });

        var request = new CreateTaskRequest
        {
            Name = "New Task",
            Description = "Build something great",
            Priority = 2,
            Assignees = [42, 99],
            Tags = ["urgent", "backend"],
            DueDate = 1700000000000L,
            DueDateTime = true,
        };

        var task = await client.CreateTaskAsync("lst_1", request);

        task.Id.Should().Be("t_new");
        task.Name.Should().Be("New Task");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/list/lst_1/task");

        var body = await handler.GetLastRequestBodyAsync();
        body.Should().Contain("New Task");
        body.Should().Contain("Build something great");
        body.Should().Contain("42");
        body.Should().Contain("urgent");
        body.Should().Contain("1700000000000");
    }

    [Fact]
    public async Task CreateTaskAsync_Omits_Null_Fields_From_Body()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "t_min", name = "Minimal" });

        var request = new CreateTaskRequest { Name = "Minimal" };

        await client.CreateTaskAsync("lst_1", request);

        var body = await handler.GetLastRequestBodyAsync();
        body.Should().NotContain("description");
        body.Should().NotContain("assignees");
        body.Should().NotContain("priority");
    }

    // ──────────────────────────────────────────
    //  UpdateTask
    // ──────────────────────────────────────────

    [Fact]
    public async Task UpdateTaskAsync_Puts_Partial_Update()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "t_1", name = "Updated Name", status = new { status = "in progress" } });

        var request = new UpdateTaskRequest
        {
            Name = "Updated Name",
            Status = "in progress",
        };

        var task = await client.UpdateTaskAsync("t_1", request);

        task.Name.Should().Be("Updated Name");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/task/t_1");
    }

    [Fact]
    public async Task UpdateTaskAsync_Supports_Assignee_Add_Remove()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "t_1", name = "Task" });

        var request = new UpdateTaskRequest
        {
            Assignees = new AssigneeUpdate
            {
                Add = [100],
                Remove = [200]
            }
        };

        await client.UpdateTaskAsync("t_1", request);

        var body = await handler.GetLastRequestBodyAsync();
        body.Should().Contain("100");
        body.Should().Contain("200");
    }

    // ──────────────────────────────────────────
    //  DeleteTask
    // ──────────────────────────────────────────

    [Fact]
    public async Task DeleteTaskAsync_Sends_Delete()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueResponse(HttpStatusCode.OK);

        await client.DeleteTaskAsync("t_1");

        handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/task/t_1");
    }

    // ──────────────────────────────────────────
    //  Custom Fields
    // ──────────────────────────────────────────

    [Fact]
    public async Task SetCustomFieldValueAsync_Posts_Value()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueResponse(HttpStatusCode.OK);

        await client.SetCustomFieldValueAsync("t_1", "cf_uuid", "dropdown_option_id");

        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/task/t_1/field/cf_uuid");

        var body = await handler.GetLastRequestBodyAsync();
        body.Should().Contain("dropdown_option_id");
    }

    [Fact]
    public async Task RemoveCustomFieldValueAsync_Sends_Delete()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueResponse(HttpStatusCode.OK);

        await client.RemoveCustomFieldValueAsync("t_1", "cf_uuid");

        handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/v2/task/t_1/field/cf_uuid");
    }

    // ──────────────────────────────────────────
    //  Error handling
    // ──────────────────────────────────────────

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task All_Methods_Throw_ClickUpApiException_On_Error(HttpStatusCode statusCode)
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueError(statusCode, """{"err":"Oops"}""");

        var act = () => client.GetTaskAsync("t_1");

        var ex = await act.Should().ThrowAsync<ClickUpApiException>();
        ex.Which.StatusCode.Should().Be(statusCode);
        ex.Which.ResponseBody.Should().Contain("Oops");
    }

    [Fact]
    public async Task CancellationToken_Is_Respected()
    {
        var (client, handler) = ClickUpClientFixture.Create();
        handler.EnqueueSuccess(new { id = "t_1", name = "Task" });

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = () => client.GetTaskAsync("t_1", ct: cts.Token);

        await act.Should().ThrowAsync<TaskCanceledException>();
    }
}
