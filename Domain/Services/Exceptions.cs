namespace Domain.Services;

public class InvalidGuidException : Exception
{
    public InvalidGuidException(string? message = null) : base(message) { }
}

// Add new exceptions here
