namespace MusicWeb.src.Exceptions;

public sealed class ErrorResponse
{
    public string Error { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string TraceId { get; init; } = string.Empty;
    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}