namespace ClickUp.Api.V2.List.Item.TaskNamespace;

public partial class TaskGetResponse_tasks
{
    public string? StatusName => Status.AsDictionary()?["status"]?.AsString();
}