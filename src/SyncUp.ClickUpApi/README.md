# SyncUp.ClickUpApi

A strongly-typed C# client library for the **ClickUp API v2**, built on `HttpClient` with `IHttpClientFactory` and dependency injection support.

## Features

- **Core endpoints**: Workspaces, Spaces, Folders, Lists, Tasks, Custom Fields
- **Strongly-typed DTOs** for all request/response payloads
- **Authentication**: Personal API tokens and OAuth 2.0 via a `DelegatingHandler`
- **Cancellation token** support on every method
- **Pagination helper** with `IAsyncEnumerable<T>` for auto-paginating tasks
- **DI-friendly**: Register with one call via `AddClickUpClient()`
- **Individual interfaces** (`IClickUpTaskService`, `IClickUpSpaceService`, etc.) for clean dependency injection
- **Aggregated facade** (`IClickUpClient`) when you want a single entry point

## Quick Start

### 1. Register in `Program.cs`

```csharp
using SyncUp.ClickUpApi.Extensions;

builder.Services.AddClickUpClient(options =>
{
    options.PersonalApiToken = "pk_YOUR_TOKEN_HERE";
});
```

Or bind from `appsettings.json`:

```json
{
  "ClickUp": {
    "PersonalApiToken": "pk_YOUR_TOKEN_HERE"
  }
}
```

```csharp
builder.Services.AddClickUpClient(builder.Configuration);
```

### 2. Inject and Use

```csharp
public class MyService
{
    private readonly IClickUpClient _clickUp;

    public MyService(IClickUpClient clickUp)
    {
        _clickUp = clickUp;
    }

    public async Task DoWorkAsync(CancellationToken ct)
    {
        // Get workspaces
        var workspaces = await _clickUp.GetWorkspacesAsync(ct);

        // Get spaces in a workspace
        var spaces = await _clickUp.GetSpacesAsync(workspaces[0].Id, ct: ct);

        // Create a task
        var task = await _clickUp.CreateTaskAsync("list_id", new()
        {
            Name = "My new task",
            Description = "Created via the API client",
            Priority = 2, // 1=Urgent, 2=High, 3=Normal, 4=Low
        }, ct);

        // Update a task
        await _clickUp.UpdateTaskAsync(task.Id, new()
        {
            Status = "in progress",
        }, ct);

        // Get a single task with subtasks
        var detail = await _clickUp.GetTaskAsync(task.Id, includeSubtasks: true, ct);
    }
}
```

### 3. Auto-Paginate Tasks

```csharp
using SyncUp.ClickUpApi.Helpers;

await foreach (var task in PaginationHelper.GetAllTasksAsync(
    _clickUp, "list_id", new() { IncludeClosed = true }, ct))
{
    Console.WriteLine($"{task.Id}: {task.Name}");
}
```

### 4. Use Individual Interfaces

You can inject only the interface you need:

```csharp
public class TaskWorker
{
    private readonly IClickUpTaskService _tasks;
    public TaskWorker(IClickUpTaskService tasks) => _tasks = tasks;
}
```

### 5. OAuth Flow

```csharp
// Exchange the authorization code
var token = await _clickUp.GetAccessTokenAsync(
    clientId: "your_client_id",
    clientSecret: "your_client_secret",
    code: "auth_code_from_redirect",
    ct);

// Now configure the client with the access token
// (typically store and reload via IOptionsMonitor)
```

## Project Structure

```
SyncUp.ClickUpApi/
├── Auth/
│   └── ClickUpAuthHandler.cs          # DelegatingHandler for auth headers
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # DI registration
├── Helpers/
│   └── PaginationHelper.cs            # IAsyncEnumerable pagination
├── Models/
│   ├── Common/
│   │   └── CommonModels.cs            # Shared types (User, Status, Tag, etc.)
│   ├── Requests/
│   │   └── RequestModels.cs           # Create/Update DTOs + query builders
│   ├── Responses/
│   │   └── ResponseModels.cs          # API response wrappers
│   ├── ClickUpTask.cs                 # Full task model
│   └── HierarchyModels.cs            # Workspace, Space, Folder, List
├── Services/
│   ├── IClickUpServices.cs            # All interfaces
│   └── ClickUpClient.cs               # Implementation
├── ClickUpApiException.cs             # Typed API exception
├── ClickUpOptions.cs                  # Configuration POCO
└── SyncUp.ClickUpApi.csproj
```

## Error Handling

All API errors throw `ClickUpApiException` with the HTTP status code and response body:

```csharp
try
{
    var task = await _clickUp.GetTaskAsync("invalid_id", ct: ct);
}
catch (ClickUpApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    // Task not found
}
catch (ClickUpApiException ex)
{
    Console.WriteLine($"API error {(int)ex.StatusCode}: {ex.ResponseBody}");
}
```

## Extending the Client

To add additional ClickUp endpoints (Comments, Goals, Time Tracking, Webhooks, etc.):

1. Define the interface in `Services/IClickUpServices.cs`
2. Add the implementation methods to `Services/ClickUpClient.cs`
3. Have `IClickUpClient` extend your new interface
4. Register the new interface in `ServiceCollectionExtensions.cs`
