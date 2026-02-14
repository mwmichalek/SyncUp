using System.Text.Json.Serialization;
using SyncUp.ClickUpApi.Models.Common;
using TaskStatus = SyncUp.ClickUpApi.Models.Common.TaskStatus;

namespace SyncUp.ClickUpApi.Models;

/// <summary>
/// Full ClickUp task object.
/// </summary>
public sealed class ClickUpTask
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("text_content")]
    public string? TextContent { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public TaskStatus? Status { get; set; }

    [JsonPropertyName("orderindex")]
    public string? Orderindex { get; set; }

    [JsonPropertyName("date_created")]
    public string? DateCreated { get; set; }

    [JsonPropertyName("date_updated")]
    public string? DateUpdated { get; set; }

    [JsonPropertyName("date_closed")]
    public string? DateClosed { get; set; }

    [JsonPropertyName("date_done")]
    public string? DateDone { get; set; }

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("creator")]
    public UserReference? Creator { get; set; }

    [JsonPropertyName("assignees")]
    public List<UserReference>? Assignees { get; set; }

    [JsonPropertyName("watchers")]
    public List<UserReference>? Watchers { get; set; }

    [JsonPropertyName("checklists")]
    public List<object>? Checklists { get; set; }

    [JsonPropertyName("tags")]
    public List<Tag>? Tags { get; set; }

    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    [JsonPropertyName("priority")]
    public TaskPriority? Priority { get; set; }

    [JsonPropertyName("due_date")]
    public string? DueDate { get; set; }

    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    [JsonPropertyName("points")]
    public double? Points { get; set; }

    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; set; }

    [JsonPropertyName("time_spent")]
    public long? TimeSpent { get; set; }

    [JsonPropertyName("custom_fields")]
    public List<CustomField>? CustomFields { get; set; }

    [JsonPropertyName("dependencies")]
    public List<object>? Dependencies { get; set; }

    [JsonPropertyName("linked_tasks")]
    public List<LinkedTask>? LinkedTasks { get; set; }

    [JsonPropertyName("team_id")]
    public string? TeamId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("list")]
    public ListReference? List { get; set; }

    [JsonPropertyName("folder")]
    public FolderReference? Folder { get; set; }

    [JsonPropertyName("space")]
    public SpaceReference? Space { get; set; }

    [JsonPropertyName("subtasks")]
    public List<ClickUpTask>? Subtasks { get; set; }
}
