namespace Domain.Services;

public class InvalidGuidException : Exception
{
    public InvalidGuidException(string? message = null) : base(message) { }
}

public class UserUnauthorizedException : Exception
{
    public UserUnauthorizedException(string? message = null) : base(message) { }
}

// Add new exceptions here
