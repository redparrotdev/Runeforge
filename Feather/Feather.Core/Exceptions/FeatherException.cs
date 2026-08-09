namespace Feather.Core.Exceptions;

/// <summary>
/// Base exception class for Feather-related exceptions.
/// </summary>
public class FeatherException : Exception
{
    public FeatherException()
    {
    }

    public FeatherException(string? message) : base(message)
    {
    }

    public FeatherException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
