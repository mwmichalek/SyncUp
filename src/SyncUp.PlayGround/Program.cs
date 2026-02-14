// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using SyncUp.ClickUp.Api;
using SyncUp.ClickUp.Api.V2.TaskNamespace.Item;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using SyncUp.ClickUp.Api.V2.List.Item.TaskNamespace;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["ClickUp:ApiKey"] ?? throw new InvalidOperationException("API Key not found in user secrets.");

Console.WriteLine("Hello, World!");

// 1. Setup Authentication (ClickUp expects "Authorization" header)
var authProvider = new ApiKeyAuthenticationProvider(
    apiKey, 
    "Authorization", 
    ApiKeyAuthenticationProvider.KeyLocation.Header
);

// 2. Setup the Request Adapter
var adapter = new HttpClientRequestAdapter(authProvider);

// 3. Instantiate your Client
var client = new ClickUpApiClient(adapter);

//var task = await client.V2.Task["86b7eh98z"].GetAsync();

var sw = Stopwatch.StartNew();


var tasks = new List<TaskGetResponse_tasks>();
int? page = 0;

while (page.HasValue)
{
    var taskResponse = await client.V2.List["901412908144"].Task.GetAsTaskGetResponseAsync((s) =>
    {
        s.QueryParameters.Subtasks = true;
        s.QueryParameters.Page = page.Value;
        s.QueryParameters.IncludeClosed = true;
    });

    if (taskResponse is { Tasks: not null })
    {
        tasks.AddRange(taskResponse.Tasks);
        page = taskResponse.LastPage.HasValue && taskResponse.LastPage.Value ? null : page + 1;
    } else 
        page = null;
}

foreach (var task in tasks.Where(t => t is { StatusName: Statuses.Queued, InitiativeTask: true }))
{
    Console.WriteLine($"Id: {task.Id}, Name: {task.Name}, Status: {task.StatusName} Parent: {task.Parent}");
}

foreach (var statusName in tasks.Select(t => t.StatusName).Distinct())
{
    Console.WriteLine($"{statusName}");
}
Console.WriteLine($"{sw.ElapsedMilliseconds} ms");