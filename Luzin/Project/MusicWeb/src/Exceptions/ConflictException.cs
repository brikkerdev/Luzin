namespace MusicWeb.src.Exceptions;

public sealed class ConflictException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string ErrorCode => "Conflict";

    public ConflictException(string message) : base(message) { }

    public ConflictException(string entity, string field, object value)
        : base($"{entity} with {field} '{value}' already exists.") { }
}