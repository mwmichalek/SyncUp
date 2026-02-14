using System.Net;
using System.Text;
using System.Text.Json;

namespace SyncUp.ClickUpApi.Tests.Fixtures;

/// <summary>
/// A test-friendly HttpMessageHandler that captures requests and returns canned responses.
/// Supports chaining multiple sequential responses via <see cref="EnqueueResponse"/>.
/// </summary>
public sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responseQueue = new();
    private readonly List<HttpRequestMessage> _sentRequests = [];

    /// <summary>All requests sent through this handler, in order.</summary>
    public IReadOnlyList<HttpRequestMessage> SentRequests => _sentRequests;

    /// <summary>The most recent request sent.</summary>
    public HttpRequestMessage? LastRequest => _sentRequests.Count > 0 ? _sentRequests[^1] : null;

    /// <summary>The request body of the most recent request, read as a string.</summary>
    public async Task<string?> GetLastRequestBodyAsync()
    {
        if (LastRequest?.Content is null) return null;
        return await LastRequest.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Enqueue a canned response. Responses are dequeued in FIFO order.
    /// If no responses are queued when a request arrives, returns 200 with empty JSON object.
    /// </summary>
    public MockHttpMessageHandler EnqueueResponse(
        HttpStatusCode statusCode,
        object? body = null,
        string contentType = "application/json")
    {
        var json = body is string s ? s : JsonSerializer.Serialize(body ?? new { });
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, contentType)
        };
        _responseQueue.Enqueue(response);
        return this;
    }

    /// <summary>Shorthand: enqueue a 200 OK response with the given body.</summary>
    public MockHttpMessageHandler EnqueueSuccess(object body) =>
        EnqueueResponse(HttpStatusCode.OK, body);

    /// <summary>Shorthand: enqueue a failure response.</summary>
    public MockHttpMessageHandler EnqueueError(
        HttpStatusCode statusCode,
        string errorBody = """{"err":"Something went wrong","ECODE":"ITEM_015"}""") =>
        EnqueueResponse(statusCode, errorBody);

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _sentRequests.Add(request);

        var response = _responseQueue.Count > 0
            ? _responseQueue.Dequeue()
            : new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };

        return Task.FromResult(response);
    }
}
