namespace MusicWeb.src.Exceptions;

public sealed class NotFoundException : AppException
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string ErrorCode => "NotFound";

    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}