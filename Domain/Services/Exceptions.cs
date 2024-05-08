using Domain.BaseTypes;

namespace Domain.Services;

/// <summary>
/// Thrown when trying to get nonexistent object from database 
/// </summary>
public class InvalidGuidException<RecordType> : Exception where RecordType : notnull, BaseEntity
{
    public InvalidGuidException(string message) : base(message) { }
    public InvalidGuidException() : base($"No {typeof(RecordType).Name} with given Guid") { }
}
public class InvalidGuidException : Exception
{
    public InvalidGuidException(string message) : base(message) { }
    public InvalidGuidException() : base($"No Entity with given Guid") { }
}

/// <summary>
/// Thrown when trying to assign empty content to an object
/// </summary>
public class EmptyContentException : Exception
{
    public EmptyContentException() : base("Content must not be empty") { }
    public EmptyContentException(string message) : base(message) { }
}

public class UserUnauthorizedException : Exception
{
    public UserUnauthorizedException(string? message = null) : base(message) { }
}
