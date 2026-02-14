using System.Text.Json.Serialization;

namespace SyncUp.ClickUpApi.Models.Requests;

// ──────────────────────────────────────────────
//  Tasks
// ──────────────────────────────────────────────

public sealed class CreateTaskRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("markdown_description")]
    public string? MarkdownDescription { get; set; }

    [JsonPropertyName("assignees")]
    public List<int>? Assignees { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("due_date")]
    public long? DueDate { get; set; }

    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    [JsonPropertyName("start_date")]
    public long? StartDate { get; set; }

    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; set; }

    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; set; }

    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; set; }

    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    [JsonPropertyName("links_to")]
    public string? LinksTo { get; set; }

    [JsonPropertyName("check_required_custom_fields")]
    public bool? CheckRequiredCustomFields { get; set; }

    [JsonPropertyName("custom_fields")]
    public List<CustomFieldValue>? CustomFields { get; set; }

    [JsonPropertyName("custom_item_id")]
    public int? CustomItemId { get; set; }
}

public sealed class UpdateTaskRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("markdown_description")]
    public string? MarkdownDescription { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("due_date")]
    public long? DueDate { get; set; }

    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    [JsonPropertyName("start_date")]
    public long? StartDate { get; set; }

    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; set; }

    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; set; }

    [JsonPropertyName("assignees")]
    public AssigneeUpdate? Assignees { get; set; }

    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    [JsonPropertyName("parent")]
    public string? Parent { get; set; }
}

public sealed class AssigneeUpdate
{
    [JsonPropertyName("add")]
    public List<int>? Add { get; set; }

    [JsonPropertyName("rem")]
    public List<int>? Remove { get; set; }
}

public sealed class CustomFieldValue
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }
}

/// <summary>
/// Query parameters for listing tasks in a list.
/// </summary>
public sealed class GetTasksQuery
{
    public bool? Archived { get; set; }
    public int? Page { get; set; }
    public string? OrderBy { get; set; }
    public bool? Reverse { get; set; }
    public bool? Subtasks { get; set; }
    public List<string>? Statuses { get; set; }
    public bool? IncludeClosed { get; set; }
    public List<int>? Assignees { get; set; }
    public List<string>? Tags { get; set; }
    public long? DueDateGt { get; set; }
    public long? DueDateLt { get; set; }
    public long? DateCreatedGt { get; set; }
    public long? DateCreatedLt { get; set; }
    public long? DateUpdatedGt { get; set; }
    public long? DateUpdatedLt { get; set; }
    public long? DateDoneGt { get; set; }
    public long? DateDoneLt { get; set; }
    public List<string>? CustomFields { get; set; }
    public bool? CustomTaskIds { get; set; }
    public long? TeamId { get; set; }

    internal Dictionary<string, string> ToQueryParameters()
    {
        var dict = new Dictionary<string, string>();

        if (Archived.HasValue) dict["archived"] = Archived.Value.ToString().ToLower();
        if (Page.HasValue) dict["page"] = Page.Value.ToString();
        if (OrderBy is not null) dict["order_by"] = OrderBy;
        if (Reverse.HasValue) dict["reverse"] = Reverse.Value.ToString().ToLower();
        if (Subtasks.HasValue) dict["subtasks"] = Subtasks.Value.ToString().ToLower();
        if (IncludeClosed.HasValue) dict["include_closed"] = IncludeClosed.Value.ToString().ToLower();
        if (DueDateGt.HasValue) dict["due_date_gt"] = DueDateGt.Value.ToString();
        if (DueDateLt.HasValue) dict["due_date_lt"] = DueDateLt.Value.ToString();
        if (DateCreatedGt.HasValue) dict["date_created_gt"] = DateCreatedGt.Value.ToString();
        if (DateCreatedLt.HasValue) dict["date_created_lt"] = DateCreatedLt.Value.ToString();
        if (DateUpdatedGt.HasValue) dict["date_updated_gt"] = DateUpdatedGt.Value.ToString();
        if (DateUpdatedLt.HasValue) dict["date_updated_lt"] = DateUpdatedLt.Value.ToString();
        if (DateDoneGt.HasValue) dict["date_done_gt"] = DateDoneGt.Value.ToString();
        if (DateDoneLt.HasValue) dict["date_done_lt"] = DateDoneLt.Value.ToString();
        if (CustomTaskIds.HasValue) dict["custom_task_ids"] = CustomTaskIds.Value.ToString().ToLower();
        if (TeamId.HasValue) dict["team_id"] = TeamId.Value.ToString();

        if (Statuses?.Count > 0)
            foreach (var s in Statuses) dict[$"statuses[]"] = s; // repeated key handled by query builder

        if (Assignees?.Count > 0)
            foreach (var a in Assignees) dict[$"assignees[]"] = a.ToString();

        if (Tags?.Count > 0)
            foreach (var t in Tags) dict[$"tags[]"] = t;

        return dict;
    }
}

// ──────────────────────────────────────────────
//  Folders
// ──────────────────────────────────────────────

public sealed class CreateFolderRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

public sealed class UpdateFolderRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

// ──────────────────────────────────────────────
//  Lists
// ──────────────────────────────────────────────

public sealed class CreateListRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("due_date")]
    public long? DueDate { get; set; }

    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public sealed class UpdateListRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("due_date")]
    public long? DueDate { get; set; }

    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("assignee")]
    public string? Assignee { get; set; }

    [JsonPropertyName("unset_status")]
    public bool? UnsetStatus { get; set; }
}

// ──────────────────────────────────────────────
//  Spaces
// ──────────────────────────────────────────────

public sealed class CreateSpaceRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("multiple_assignees")]
    public bool? MultipleAssignees { get; set; }

    [JsonPropertyName("features")]
    public SpaceFeaturesRequest? Features { get; set; }
}

public sealed class UpdateSpaceRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("private")]
    public bool? Private { get; set; }

    [JsonPropertyName("admin_can_manage")]
    public bool? AdminCanManage { get; set; }

    [JsonPropertyName("multiple_assignees")]
    public bool? MultipleAssignees { get; set; }

    [JsonPropertyName("features")]
    public SpaceFeaturesRequest? Features { get; set; }
}

public sealed class SpaceFeaturesRequest
{
    [JsonPropertyName("due_dates")]
    public FeatureToggle? DueDates { get; set; }

    [JsonPropertyName("time_tracking")]
    public FeatureToggle? TimeTracking { get; set; }

    [JsonPropertyName("tags")]
    public FeatureToggle? Tags { get; set; }

    [JsonPropertyName("time_estimates")]
    public FeatureToggle? TimeEstimates { get; set; }

    [JsonPropertyName("checklists")]
    public FeatureToggle? Checklists { get; set; }

    [JsonPropertyName("custom_fields")]
    public FeatureToggle? CustomFields { get; set; }

    [JsonPropertyName("remap_dependencies")]
    public FeatureToggle? RemapDependencies { get; set; }

    [JsonPropertyName("dependency_warning")]
    public FeatureToggle? DependencyWarning { get; set; }

    [JsonPropertyName("portfolios")]
    public FeatureToggle? Portfolios { get; set; }
}

public sealed class FeatureToggle
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}
