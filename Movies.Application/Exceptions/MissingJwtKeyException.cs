namespace Movies.Application.Exceptions;
public class MissingJwtKeyException : Exception
{
    public MissingJwtKeyException()
        : base("JWT key is not configured.") { }

    public MissingJwtKeyException(string? message)
        : base(message) { }

    public MissingJwtKeyException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
