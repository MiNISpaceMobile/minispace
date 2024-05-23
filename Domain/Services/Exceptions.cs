using Domain.BaseTypes;

namespace Domain.Services;

/// <summary>
/// Base type for Minispace custom exceptions
/// </summary>
public abstract class MinispaceException(string message) : Exception(message) { }

/// <summary>
/// Thrown when trying to get nonexistent object from database
/// </summary>
public class InvalidGuidException : MinispaceException
{
    public InvalidGuidException(string message) : base(message) { }
    public InvalidGuidException() : base($"No Entity with given Guid exists") { }
}
/// <summary>
/// Thrown when trying to get nonexistent object from database
/// </summary>
public class InvalidGuidException<RecordType> : InvalidGuidException where RecordType : notnull, BaseEntity
{
    public InvalidGuidException(string message) : base(message) { }
    public InvalidGuidException() : base($"No {typeof(RecordType).Name} with given Guid exists") { }
}

/// <summary>
/// Thrown when trying to use service method while unathorized
/// </summary>
public class UserUnauthorizedException : MinispaceException
{
    public UserUnauthorizedException(string message) : base(message) { }
    public UserUnauthorizedException() : base("Acting user is not authorized to perform this action") { }
}

/// <summary>
/// Thrown when trying to assign empty content to an object
/// </summary>
public class EmptyContentException : MinispaceException
{
    public EmptyContentException() : base("Content must not be empty") { }
    public EmptyContentException(string message) : base(message) { }
}

public class FriendTargetException : MinispaceException
{
    public FriendTargetException() : base("This user cannot be befriended") { }
    public FriendTargetException(string message) : base(message) { }
}