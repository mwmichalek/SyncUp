using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SyncUp.ClickUpApi.Models;
using SyncUp.ClickUpApi.Models.Common;
using SyncUp.ClickUpApi.Models.Requests;
using SyncUp.ClickUpApi.Models.Responses;

namespace SyncUp.ClickUpApi.Services;

/// <summary>
/// Full implementation of the ClickUp API v2 client covering core endpoints:
/// Auth, Workspaces, Spaces, Folders, Lists, and Tasks.
/// </summary>
public sealed class ClickUpClient : IClickUpClient
{
    private readonly HttpClient _http;
    private readonly IOptionsMonitor<ClickUpOptions> _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    public ClickUpClient(HttpClient http, IOptionsMonitor<ClickUpOptions> options)
    {
        _http = http;
        _options = options;
    }

    // ──────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────

    private string Base => _options.CurrentValue.BaseUrl.TrimEnd('/');

    private async Task<T> GetAsync<T>(string url, CancellationToken ct)
    {
        using var response = await _http.GetAsync(url, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    private async Task<T> PostAsync<T>(string url, object body, CancellationToken ct)
    {
        using var content = Serialize(body);
        using var response = await _http.PostAsync(url, content, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    private async Task PostAsync(string url, object body, CancellationToken ct)
    {
        using var content = Serialize(body);
        using var response = await _http.PostAsync(url, content, ct);
        await EnsureSuccessAsync(response, ct);
    }

    private async Task<T> PutAsync<T>(string url, object body, CancellationToken ct)
    {
        using var content = Serialize(body);
        using var response = await _http.PutAsync(url, content, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    private async Task PutAsync(string url, object body, CancellationToken ct)
    {
        using var content = Serialize(body);
        using var response = await _http.PutAsync(url, content, ct);
        await EnsureSuccessAsync(response, ct);
    }

    private async Task DeleteAsync(string url, CancellationToken ct)
    {
        using var response = await _http.DeleteAsync(url, ct);
        await EnsureSuccessAsync(response, ct);
    }

    private static StringContent Serialize(object body) =>
        new(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            throw new ClickUpApiException(response.StatusCode, errorBody);
        }

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
        return result ?? throw new ClickUpApiException(
            response.StatusCode, "Response deserialized to null.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            throw new ClickUpApiException(response.StatusCode, errorBody);
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        if (parameters.Count == 0) return string.Empty;
        var sb = new StringBuilder("?");
        var first = true;
        foreach (var (key, value) in parameters)
        {
            if (!first) sb.Append('&');
            sb.Append(Uri.EscapeDataString(key)).Append('=').Append(Uri.EscapeDataString(value));
            first = false;
        }
        return sb.ToString();
    }

    private static string BuildQueryString(params (string key, string? value)[] parameters)
    {
        var dict = new Dictionary<string, string>();
        foreach (var (key, value) in parameters)
        {
            if (value is not null) dict[key] = value;
        }
        return BuildQueryString(dict);
    }

    // ──────────────────────────────────────────
    //  Auth / User
    // ──────────────────────────────────────────

    public async Task<AccessTokenResponse> GetAccessTokenAsync(
        string clientId, string clientSecret, string code, CancellationToken ct = default)
    {
        var body = new { client_id = clientId, client_secret = clientSecret, code };
        return await PostAsync<AccessTokenResponse>($"{Base}/v2/oauth/token", body, ct);
    }

    public async Task<ClickUpUser> GetAuthorizedUserAsync(CancellationToken ct = default)
    {
        var response = await GetAsync<AuthorizedUserResponse>($"{Base}/v2/user", ct);
        return response.User;
    }

    // ──────────────────────────────────────────
    //  Workspaces
    // ──────────────────────────────────────────

    public async Task<List<Workspace>> GetWorkspacesAsync(CancellationToken ct = default)
    {
        var response = await GetAsync<GetWorkspacesResponse>($"{Base}/v2/team", ct);
        return response.Teams;
    }

    // ──────────────────────────────────────────
    //  Spaces
    // ──────────────────────────────────────────

    public async Task<List<Space>> GetSpacesAsync(
        string workspaceId, bool archived = false, CancellationToken ct = default)
    {
        var qs = BuildQueryString(("archived", archived.ToString().ToLower()));
        var response = await GetAsync<GetSpacesResponse>($"{Base}/v2/team/{workspaceId}/space{qs}", ct);
        return response.Spaces;
    }

    public Task<Space> GetSpaceAsync(string spaceId, CancellationToken ct = default) =>
        GetAsync<Space>($"{Base}/v2/space/{spaceId}", ct);

    public Task<Space> CreateSpaceAsync(
        string workspaceId, CreateSpaceRequest request, CancellationToken ct = default) =>
        PostAsync<Space>($"{Base}/v2/team/{workspaceId}/space", request, ct);

    public Task<Space> UpdateSpaceAsync(
        string spaceId, UpdateSpaceRequest request, CancellationToken ct = default) =>
        PutAsync<Space>($"{Base}/v2/space/{spaceId}", request, ct);

    public Task DeleteSpaceAsync(string spaceId, CancellationToken ct = default) =>
        DeleteAsync($"{Base}/v2/space/{spaceId}", ct);

    // ──────────────────────────────────────────
    //  Folders
    // ──────────────────────────────────────────

    public async Task<List<Folder>> GetFoldersAsync(
        string spaceId, bool archived = false, CancellationToken ct = default)
    {
        var qs = BuildQueryString(("archived", archived.ToString().ToLower()));
        var response = await GetAsync<GetFoldersResponse>($"{Base}/v2/space/{spaceId}/folder{qs}", ct);
        return response.Folders;
    }

    public Task<Folder> GetFolderAsync(string folderId, CancellationToken ct = default) =>
        GetAsync<Folder>($"{Base}/v2/folder/{folderId}", ct);

    public Task<Folder> CreateFolderAsync(
        string spaceId, CreateFolderRequest request, CancellationToken ct = default) =>
        PostAsync<Folder>($"{Base}/v2/space/{spaceId}/folder", request, ct);

    public Task<Folder> UpdateFolderAsync(
        string folderId, UpdateFolderRequest request, CancellationToken ct = default) =>
        PutAsync<Folder>($"{Base}/v2/folder/{folderId}", request, ct);

    public Task DeleteFolderAsync(string folderId, CancellationToken ct = default) =>
        DeleteAsync($"{Base}/v2/folder/{folderId}", ct);

    // ──────────────────────────────────────────
    //  Lists
    // ──────────────────────────────────────────

    public async Task<List<ClickUpList>> GetListsAsync(
        string folderId, bool archived = false, CancellationToken ct = default)
    {
        var qs = BuildQueryString(("archived", archived.ToString().ToLower()));
        var response = await GetAsync<GetListsResponse>($"{Base}/v2/folder/{folderId}/list{qs}", ct);
        return response.Lists;
    }

    public async Task<List<ClickUpList>> GetFolderlessListsAsync(
        string spaceId, bool archived = false, CancellationToken ct = default)
    {
        var qs = BuildQueryString(("archived", archived.ToString().ToLower()));
        var response = await GetAsync<GetListsResponse>($"{Base}/v2/space/{spaceId}/list{qs}", ct);
        return response.Lists;
    }

    public Task<ClickUpList> GetListAsync(string listId, CancellationToken ct = default) =>
        GetAsync<ClickUpList>($"{Base}/v2/list/{listId}", ct);

    public Task<ClickUpList> CreateListAsync(
        string folderId, CreateListRequest request, CancellationToken ct = default) =>
        PostAsync<ClickUpList>($"{Base}/v2/folder/{folderId}/list", request, ct);

    public Task<ClickUpList> CreateFolderlessListAsync(
        string spaceId, CreateListRequest request, CancellationToken ct = default) =>
        PostAsync<ClickUpList>($"{Base}/v2/space/{spaceId}/list", request, ct);

    public Task<ClickUpList> UpdateListAsync(
        string listId, UpdateListRequest request, CancellationToken ct = default) =>
        PutAsync<ClickUpList>($"{Base}/v2/list/{listId}", request, ct);

    public Task DeleteListAsync(string listId, CancellationToken ct = default) =>
        DeleteAsync($"{Base}/v2/list/{listId}", ct);

    public Task<GetCustomFieldsResponse> GetCustomFieldsAsync(
        string listId, CancellationToken ct = default) =>
        GetAsync<GetCustomFieldsResponse>($"{Base}/v2/list/{listId}/field", ct);

    // ──────────────────────────────────────────
    //  Tasks
    // ──────────────────────────────────────────

    public async Task<GetTasksResponse> GetTasksAsync(
        string listId, GetTasksQuery? query = null, CancellationToken ct = default)
    {
        var qs = query is not null ? BuildQueryString(query.ToQueryParameters()) : string.Empty;
        return await GetAsync<GetTasksResponse>($"{Base}/v2/list/{listId}/task{qs}", ct);
    }

    public Task<ClickUpTask> GetTaskAsync(
        string taskId, bool? includeSubtasks = null, CancellationToken ct = default)
    {
        var qs = includeSubtasks.HasValue
            ? BuildQueryString(("include_subtasks", includeSubtasks.Value.ToString().ToLower()))
            : string.Empty;
        return GetAsync<ClickUpTask>($"{Base}/v2/task/{taskId}{qs}", ct);
    }

    public Task<ClickUpTask> CreateTaskAsync(
        string listId, CreateTaskRequest request, CancellationToken ct = default) =>
        PostAsync<ClickUpTask>($"{Base}/v2/list/{listId}/task", request, ct);

    public Task<ClickUpTask> UpdateTaskAsync(
        string taskId, UpdateTaskRequest request, CancellationToken ct = default) =>
        PutAsync<ClickUpTask>($"{Base}/v2/task/{taskId}", request, ct);

    public Task DeleteTaskAsync(string taskId, CancellationToken ct = default) =>
        DeleteAsync($"{Base}/v2/task/{taskId}", ct);

    public Task SetCustomFieldValueAsync(
        string taskId, string fieldId, object value, CancellationToken ct = default) =>
        PostAsync($"{Base}/v2/task/{taskId}/field/{fieldId}", new { value }, ct);

    public Task RemoveCustomFieldValueAsync(
        string taskId, string fieldId, CancellationToken ct = default) =>
        DeleteAsync($"{Base}/v2/task/{taskId}/field/{fieldId}", ct);
}
