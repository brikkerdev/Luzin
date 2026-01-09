namespace MusicWeb.src.Exceptions;

public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string ErrorCode { get; }

    protected AppException(string message) : base(message) { }
    protected AppException(string message, Exception inner) : base(message, inner) { }
}