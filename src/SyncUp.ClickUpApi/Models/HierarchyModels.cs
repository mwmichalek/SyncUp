using System.Text.Json.Serialization;
using SyncUp.ClickUpApi.Models.Common;
using TaskStatus = SyncUp.ClickUpApi.Models.Common.TaskStatus;

namespace SyncUp.ClickUpApi.Models;

// ──────────────────────────────────────────────
//  Workspace (Team)
// ──────────────────────────────────────────────

public sealed class Workspace
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("members")]
    public List<Member>? Members { get; set; }
}

// ──────────────────────────────────────────────
//  Space
// ──────────────────────────────────────────────

public sealed class Space
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("private")]
    public bool Private { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("admin_can_manage")]
    public bool? AdminCanManage { get; set; }

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("statuses")]
    public List<TaskStatus>? Statuses { get; set; }

    [JsonPropertyName("multiple_assignees")]
    public bool? MultipleAssignees { get; set; }

    [JsonPropertyName("features")]
    public SpaceFeatures? Features { get; set; }

    [JsonPropertyName("members")]
    public List<Member>? Members { get; set; }
}

public sealed class SpaceFeatures
{
    [JsonPropertyName("due_dates")]
    public FeatureConfig? DueDates { get; set; }

    [JsonPropertyName("time_tracking")]
    public FeatureConfig? TimeTracking { get; set; }

    [JsonPropertyName("tags")]
    public FeatureConfig? Tags { get; set; }

    [JsonPropertyName("time_estimates")]
    public FeatureConfig? TimeEstimates { get; set; }

    [JsonPropertyName("checklists")]
    public FeatureConfig? Checklists { get; set; }

    [JsonPropertyName("custom_fields")]
    public FeatureConfig? CustomFields { get; set; }

    [JsonPropertyName("remap_dependencies")]
    public FeatureConfig? RemapDependencies { get; set; }

    [JsonPropertyName("dependency_warning")]
    public FeatureConfig? DependencyWarning { get; set; }

    [JsonPropertyName("portfolios")]
    public FeatureConfig? Portfolios { get; set; }
}

public sealed class FeatureConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}

// ──────────────────────────────────────────────
//  Folder
// ──────────────────────────────────────────────

public sealed class Folder
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("orderindex")]
    public int Orderindex { get; set; }

    [JsonPropertyName("override_statuses")]
    public bool OverrideStatuses { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    [JsonPropertyName("space")]
    public SpaceReference? Space { get; set; }

    [JsonPropertyName("task_count")]
    public string? TaskCount { get; set; }

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("statuses")]
    public List<TaskStatus>? Statuses { get; set; }

    [JsonPropertyName("lists")]
    public List<ClickUpList>? Lists { get; set; }
}

// ──────────────────────────────────────────────
//  List
// ──────────────────────────────────────────────

public sealed class ClickUpList
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("orderindex")]
    public int Orderindex { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("status")]
    public TaskStatus? Status { get; set; }

    [JsonPropertyName("priority")]
    public TaskPriority? Priority { get; set; }

    [JsonPropertyName("assignee")]
    public UserReference? Assignee { get; set; }

    [JsonPropertyName("task_count")]
    public int? TaskCount { get; set; }

    [JsonPropertyName("due_date")]
    public string? DueDate { get; set; }

    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("override_statuses")]
    public bool? OverrideStatuses { get; set; }

    [JsonPropertyName("statuses")]
    public List<TaskStatus>? Statuses { get; set; }

    [JsonPropertyName("folder")]
    public FolderReference? Folder { get; set; }

    [JsonPropertyName("space")]
    public SpaceReference? Space { get; set; }

    [JsonPropertyName("permission_level")]
    public string? PermissionLevel { get; set; }
}
