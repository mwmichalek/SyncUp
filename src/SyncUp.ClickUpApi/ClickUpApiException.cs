using System.Net;

namespace SyncUp.ClickUpApi;

/// <summary>
/// Exception thrown when the ClickUp API returns a non-success status code.
/// </summary>
public sealed class ClickUpApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseBody { get; }

    public ClickUpApiException(HttpStatusCode statusCode, string? responseBody)
        : base($"ClickUp API returned {(int)statusCode} ({statusCode}): {Truncate(responseBody, 500)}")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public ClickUpApiException(HttpStatusCode statusCode, string? responseBody, Exception inner)
        : base($"ClickUp API returned {(int)statusCode} ({statusCode}): {Truncate(responseBody, 500)}", inner)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    private static string? Truncate(string? value, int maxLength) =>
        value is not null && value.Length > maxLength
            ? string.Concat(value.AsSpan(0, maxLength), "...")
            : value;
}
