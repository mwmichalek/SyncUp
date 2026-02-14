using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SyncUp.ClickUpApi.Helpers;
using SyncUp.ClickUpApi.Models;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Models.Responses;

namespace SyncUp.ClickUpApi.Tests.Helpers;

public class PaginationHelperTests
{
    [Fact]
    public async Task GetAllTasksAsync_Iterates_All_Pages()
    {
        var taskService = Substitute.For<IClickUpTaskService>();

        // Page 0: 2 tasks, not last page
        taskService.GetTasksAsync("lst_1", Arg.Is<GetTasksQuery>(q => q.Page == 0), Arg.Any<CancellationToken>())
            .Returns(new GetTasksResponse
            {
                Tasks = [new ClickUpTask { Id = "t_1", Name = "Task 1" }, new ClickUpTask { Id = "t_2", Name = "Task 2" }],
                LastPage = false
            });

        // Page 1: 1 task, last page
        taskService.GetTasksAsync("lst_1", Arg.Is<GetTasksQuery>(q => q.Page == 1), Arg.Any<CancellationToken>())
            .Returns(new GetTasksResponse
            {
                Tasks = [new ClickUpTask { Id = "t_3", Name = "Task 3" }],
                LastPage = true
            });

        var allTasks = new List<ClickUpTask>();
        await foreach (var task in PaginationHelper.GetAllTasksAsync(taskService, "lst_1"))
        {
            allTasks.Add(task);
        }

        allTasks.Should().HaveCount(3);
        allTasks.Select(t => t.Id).Should().ContainInOrder("t_1", "t_2", "t_3");
    }

    [Fact]
    public async Task GetAllTasksAsync_Stops_On_Empty_Page()
    {
        var taskService = Substitute.For<IClickUpTaskService>();

        taskService.GetTasksAsync("lst_1", Arg.Any<GetTasksQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetTasksResponse
            {
                Tasks = [],
                LastPage = false // even though LastPage is false, empty list should stop
            });

        var allTasks = new List<ClickUpTask>();
        await foreach (var task in PaginationHelper.GetAllTasksAsync(taskService, "lst_1"))
        {
            allTasks.Add(task);
        }

        allTasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllTasksAsync_Single_Page_Works()
    {
        var taskService = Substitute.For<IClickUpTaskService>();

        taskService.GetTasksAsync("lst_1", Arg.Any<GetTasksQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetTasksResponse
            {
                Tasks = [new ClickUpTask { Id = "t_solo", Name = "Only One" }],
                LastPage = true
            });

        var allTasks = new List<ClickUpTask>();
        await foreach (var task in PaginationHelper.GetAllTasksAsync(taskService, "lst_1"))
        {
            allTasks.Add(task);
        }

        allTasks.Should().HaveCount(1);
        allTasks[0].Name.Should().Be("Only One");
    }

    [Fact]
    public async Task GetAllTasksAsync_Passes_BaseQuery_Parameters()
    {
        var taskService = Substitute.For<IClickUpTaskService>();

        taskService.GetTasksAsync("lst_1", Arg.Any<GetTasksQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetTasksResponse { Tasks = [], LastPage = true });

        var baseQuery = new GetTasksQuery
        {
            IncludeClosed = true,
            OrderBy = "created",
            Subtasks = true,
        };

        await foreach (var _ in PaginationHelper.GetAllTasksAsync(taskService, "lst_1", baseQuery))
        {
            // consume
        }

        await taskService.Received(1).GetTasksAsync(
            "lst_1",
            Arg.Is<GetTasksQuery>(q =>
                q.IncludeClosed == true &&
                q.OrderBy == "created" &&
                q.Subtasks == true &&
                q.Page == 0),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllTasksAsync_Supports_Early_Break()
    {
        var taskService = Substitute.For<IClickUpTaskService>();

        taskService.GetTasksAsync("lst_1", Arg.Is<GetTasksQuery>(q => q.Page == 0), Arg.Any<CancellationToken>())
            .Returns(new GetTasksResponse
            {
                Tasks =
                [
                    new ClickUpTask { Id = "t_1" },
                    new ClickUpTask { Id = "t_2" },
                    new ClickUpTask { Id = "t_3" }
                ],
                LastPage = false
            });

        var collected = new List<string>();
        await foreach (var task in PaginationHelper.GetAllTasksAsync(taskService, "lst_1"))
        {
            collected.Add(task.Id);
            if (collected.Count == 2) break; // early exit
        }

        collected.Should().HaveCount(2);
        // Page 1 should never have been requested
        await taskService.DidNotReceive().GetTasksAsync(
            "lst_1",
            Arg.Is<GetTasksQuery>(q => q.Page == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllTasksAsync_Respects_CancellationToken()
    {
        var taskService = Substitute.For<IClickUpTaskService>();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        taskService.GetTasksAsync("lst_1", Arg.Any<GetTasksQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var act = async () =>
        {
            await foreach (var _ in PaginationHelper.GetAllTasksAsync(
                taskService, "lst_1", cancellationToken: cts.Token))
            {
            }
        };

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
