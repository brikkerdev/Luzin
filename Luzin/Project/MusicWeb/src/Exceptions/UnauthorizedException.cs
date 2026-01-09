namespace MusicWeb.src.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string ErrorCode => "Unauthorized";

    public UnauthorizedException(string message = "Authentication required.")
        : base(message) { }
}