using System.Text.Json.Serialization;
using SyncUp.ClickUpApi.Models.Common;

namespace SyncUp.ClickUpApi.Models.Responses;

// ──────────────────────────────────────────────
//  Auth
// ──────────────────────────────────────────────

public sealed class AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}

public sealed class AuthorizedUserResponse
{
    [JsonPropertyName("user")]
    public ClickUpUser User { get; set; } = new();
}

// ──────────────────────────────────────────────
//  Workspaces
// ──────────────────────────────────────────────

public sealed class GetWorkspacesResponse
{
    [JsonPropertyName("teams")]
    public List<Workspace> Teams { get; set; } = [];
}

// ──────────────────────────────────────────────
//  Spaces
// ──────────────────────────────────────────────

public sealed class GetSpacesResponse
{
    [JsonPropertyName("spaces")]
    public List<Space> Spaces { get; set; } = [];
}

// ──────────────────────────────────────────────
//  Folders
// ──────────────────────────────────────────────

public sealed class GetFoldersResponse
{
    [JsonPropertyName("folders")]
    public List<Folder> Folders { get; set; } = [];
}

// ──────────────────────────────────────────────
//  Lists
// ──────────────────────────────────────────────

public sealed class GetListsResponse
{
    [JsonPropertyName("lists")]
    public List<ClickUpList> Lists { get; set; } = [];
}

// ──────────────────────────────────────────────
//  Tasks
// ──────────────────────────────────────────────

public sealed class GetTasksResponse
{
    [JsonPropertyName("tasks")]
    public List<ClickUpTask> Tasks { get; set; } = [];

    /// <summary>
    /// True if there are more pages of tasks. Use page parameter to paginate.
    /// </summary>
    [JsonPropertyName("last_page")]
    public bool LastPage { get; set; }
}

// ──────────────────────────────────────────────
//  Custom Fields
// ──────────────────────────────────────────────

public sealed class GetCustomFieldsResponse
{
    [JsonPropertyName("fields")]
    public List<CustomField> Fields { get; set; } = [];
}
