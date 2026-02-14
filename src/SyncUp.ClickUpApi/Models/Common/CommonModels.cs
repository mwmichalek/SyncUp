using System.Text.Json.Serialization;

namespace SyncUp.ClickUpApi.Models.Common;

/// <summary>
/// Represents a ClickUp user.
/// </summary>
public sealed class ClickUpUser
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("profilePicture")]
    public string? ProfilePicture { get; set; }

    [JsonPropertyName("initials")]
    public string? Initials { get; set; }

    [JsonPropertyName("week_start_day")]
    public int? WeekStartDay { get; set; }

    [JsonPropertyName("global_font_support")]
    public bool? GlobalFontSupport { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}

/// <summary>
/// Reference to a ClickUp user (subset of fields).
/// </summary>
public sealed class UserReference
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("initials")]
    public string? Initials { get; set; }

    [JsonPropertyName("profilePicture")]
    public string? ProfilePicture { get; set; }
}

/// <summary>
/// Task status information.
/// </summary>
public sealed class TaskStatus
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("orderindex")]
    public int Orderindex { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>
/// Priority information for a task.
/// </summary>
public sealed class TaskPriority
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("orderindex")]
    public string? Orderindex { get; set; }
}

/// <summary>
/// Lightweight reference to a list.
/// </summary>
public sealed class ListReference
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("access")]
    public bool? Access { get; set; }
}

/// <summary>
/// Lightweight reference to a folder.
/// </summary>
public sealed class FolderReference
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("hidden")]
    public bool? Hidden { get; set; }

    [JsonPropertyName("access")]
    public bool? Access { get; set; }
}

/// <summary>
/// Lightweight reference to a space.
/// </summary>
public sealed class SpaceReference
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("access")]
    public bool? Access { get; set; }
}

/// <summary>
/// Tag information.
/// </summary>
public sealed class Tag
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("tag_fg")]
    public string? TagForeground { get; set; }

    [JsonPropertyName("tag_bg")]
    public string? TagBackground { get; set; }

    [JsonPropertyName("creator")]
    public int? Creator { get; set; }
}

/// <summary>
/// Custom field on a task.
/// </summary>
public sealed class CustomField
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("type_config")]
    public CustomFieldTypeConfig? TypeConfig { get; set; }

    [JsonPropertyName("date_created")]
    public string? DateCreated { get; set; }

    [JsonPropertyName("hide_from_guests")]
    public bool? HideFromGuests { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}

/// <summary>
/// Configuration details for a custom field type.
/// </summary>
public sealed class CustomFieldTypeConfig
{
    [JsonPropertyName("options")]
    public List<CustomFieldOption>? Options { get; set; }

    [JsonPropertyName("default")]
    public object? Default { get; set; }

    [JsonPropertyName("precision")]
    public int? Precision { get; set; }

    [JsonPropertyName("currency_type")]
    public string? CurrencyType { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }
}

/// <summary>
/// An option within a custom field (dropdown, label, etc.).
/// </summary>
public sealed class CustomFieldOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("orderindex")]
    public int? Orderindex { get; set; }
}

/// <summary>
/// Linked task reference.
/// </summary>
public sealed class LinkedTask
{
    [JsonPropertyName("task_id")]
    public string TaskId { get; set; } = string.Empty;

    [JsonPropertyName("link_id")]
    public string LinkId { get; set; } = string.Empty;

    [JsonPropertyName("date_created")]
    public string? DateCreated { get; set; }

    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}

/// <summary>
/// Workspace member wrapper.
/// </summary>
public sealed class Member
{
    [JsonPropertyName("user")]
    public ClickUpUser User { get; set; } = new();
}
