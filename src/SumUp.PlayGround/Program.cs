// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ClickUp.Api;
using ClickUp.Api.V2.TaskNamespace.Item;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;

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

var taskResponse = await client.V2.List["901412908144"].Task.GetAsTaskGetResponseAsync((s) =>
{
    s.QueryParameters.Subtasks = true;
    //s.Options.Add(WithTask_GetResponse_status);
});
//var taskRequest = client.V2.List["901412908144"].Task.ToGetRequestInformation();


var tasks = taskResponse.Tasks;

foreach (var task in tasks)
{
    Console.WriteLine($"Id: {task.Id}, Name: {task.Name}, Status: {task.StatusName} Parent: {task.Parent}");

    //var status = task.Status.AsDictionary()?["status"]?.AsString();
    
    // if (task.Status is UntypedObject untypedObject)
    // {
    //     var fields = untypedObject.GetValue();
    //     if (fields.TryGetValue("status", out var statusNode) && statusNode is UntypedString statusString)
    //     {
    //         var status = statusString.GetValue();
    //     }
    // }
    
    // if (task.Status is UntypedObject untypedObject && 
    //     untypedObject.GetValue().TryGetValue("status", out var statusNode) &&
    //     statusNode is UntypedString untypedStringValue)
    // {
    //     var status = untypedStringValue.GetValue();
    // }
}
Console.WriteLine($"{sw.ElapsedMilliseconds} ms");