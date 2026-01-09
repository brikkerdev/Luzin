namespace MusicWeb.src.Exceptions;

public sealed class BadRequestException : AppException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string ErrorCode => "BadRequest";

    public BadRequestException(string message) : base(message) { }
}