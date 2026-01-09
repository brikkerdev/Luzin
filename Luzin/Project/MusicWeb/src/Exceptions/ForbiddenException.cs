namespace MusicWeb.src.Exceptions;

public sealed class ForbiddenException : AppException
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string ErrorCode => "Forbidden";

    public ForbiddenException(string message = "Access denied.")
        : base(message) { }
}