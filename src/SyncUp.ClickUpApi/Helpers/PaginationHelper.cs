using System.Runtime.CompilerServices;

namespace SyncUp.ClickUpApi.Helpers;

/// <summary>
/// Helpers for auto-paginating through ClickUp API results.
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Enumerates all tasks across pages for a given list.
    /// Yields tasks one at a time so the caller can break early.
    /// </summary>
    public static async IAsyncEnumerable<Models.ClickUpTask> GetAllTasksAsync(
        IClickUpTaskService taskService,
        string listId,
        Models.Requests.GetTasksQuery? baseQuery = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = baseQuery ?? new Models.Requests.GetTasksQuery();
        var page = 0;

        while (true)
        {
            query.Page = page;
            var response = await taskService.GetTasksAsync(listId, query, cancellationToken);

            foreach (var task in response.Tasks)
            {
                yield return task;
            }

            if (response.LastPage || response.Tasks.Count == 0)
                yield break;

            page++;
        }
    }
}
