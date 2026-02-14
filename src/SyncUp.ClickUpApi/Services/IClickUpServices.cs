using SyncUp.ClickUpApi.Models;
using SyncUp.ClickUpApi.Models.Common;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Models.Responses;

namespace SyncUp.ClickUpApi;

// ──────────────────────────────────────────────
//  Auth / User
// ──────────────────────────────────────────────

public interface IClickUpAuthService
{
    /// <summary>Exchange an OAuth authorization code for an access token.</summary>
    Task<AccessTokenResponse> GetAccessTokenAsync(
        string clientId, string clientSecret, string code, CancellationToken ct = default);

    /// <summary>Get the authenticated user's details.</summary>
    Task<ClickUpUser> GetAuthorizedUserAsync(CancellationToken ct = default);
}

// ──────────────────────────────────────────────
//  Workspaces (Teams)
// ──────────────────────────────────────────────

public interface IClickUpWorkspaceService
{
    Task<List<Workspace>> GetWorkspacesAsync(CancellationToken ct = default);
}

// ──────────────────────────────────────────────
//  Spaces
// ──────────────────────────────────────────────

public interface IClickUpSpaceService
{
    Task<List<Space>> GetSpacesAsync(string workspaceId, bool archived = false, CancellationToken ct = default);
    Task<Space> GetSpaceAsync(string spaceId, CancellationToken ct = default);
    Task<Space> CreateSpaceAsync(string workspaceId, CreateSpaceRequest request, CancellationToken ct = default);
    Task<Space> UpdateSpaceAsync(string spaceId, UpdateSpaceRequest request, CancellationToken ct = default);
    Task DeleteSpaceAsync(string spaceId, CancellationToken ct = default);
}

// ──────────────────────────────────────────────
//  Folders
// ──────────────────────────────────────────────

public interface IClickUpFolderService
{
    Task<List<Folder>> GetFoldersAsync(string spaceId, bool archived = false, CancellationToken ct = default);
    Task<Folder> GetFolderAsync(string folderId, CancellationToken ct = default);
    Task<Folder> CreateFolderAsync(string spaceId, CreateFolderRequest request, CancellationToken ct = default);
    Task<Folder> UpdateFolderAsync(string folderId, UpdateFolderRequest request, CancellationToken ct = default);
    Task DeleteFolderAsync(string folderId, CancellationToken ct = default);
}

// ──────────────────────────────────────────────
//  Lists
// ──────────────────────────────────────────────

public interface IClickUpListService
{
    Task<List<ClickUpList>> GetListsAsync(string folderId, bool archived = false, CancellationToken ct = default);
    Task<List<ClickUpList>> GetFolderlessListsAsync(string spaceId, bool archived = false, CancellationToken ct = default);
    Task<ClickUpList> GetListAsync(string listId, CancellationToken ct = default);
    Task<ClickUpList> CreateListAsync(string folderId, CreateListRequest request, CancellationToken ct = default);
    Task<ClickUpList> CreateFolderlessListAsync(string spaceId, CreateListRequest request, CancellationToken ct = default);
    Task<ClickUpList> UpdateListAsync(string listId, UpdateListRequest request, CancellationToken ct = default);
    Task DeleteListAsync(string listId, CancellationToken ct = default);
    Task<GetCustomFieldsResponse> GetCustomFieldsAsync(string listId, CancellationToken ct = default);
}

// ──────────────────────────────────────────────
//  Tasks
// ──────────────────────────────────────────────

public interface IClickUpTaskService
{
    Task<GetTasksResponse> GetTasksAsync(string listId, GetTasksQuery? query = null, CancellationToken ct = default);
    Task<ClickUpTask> GetTaskAsync(string taskId, bool? includeSubtasks = null, CancellationToken ct = default);
    Task<ClickUpTask> CreateTaskAsync(string listId, CreateTaskRequest request, CancellationToken ct = default);
    Task<ClickUpTask> UpdateTaskAsync(string taskId, UpdateTaskRequest request, CancellationToken ct = default);
    Task DeleteTaskAsync(string taskId, CancellationToken ct = default);
    Task SetCustomFieldValueAsync(string taskId, string fieldId, object value, CancellationToken ct = default);
    Task RemoveCustomFieldValueAsync(string taskId, string fieldId, CancellationToken ct = default);
}

// ──────────────────────────────────────────────
//  Aggregated facade
// ──────────────────────────────────────────────

/// <summary>
/// Single entry point that aggregates all ClickUp service interfaces.
/// </summary>
public interface IClickUpClient :
    IClickUpAuthService,
    IClickUpWorkspaceService,
    IClickUpSpaceService,
    IClickUpFolderService,
    IClickUpListService,
    IClickUpTaskService
{
}
