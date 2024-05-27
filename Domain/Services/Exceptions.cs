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

/// <summary>
/// Thrown when trying to upload a file (picture), that is too big
/// </summary>
public class FileTooBigException : MinispaceException
{
    public FileTooBigException() : base("File is too big") { }
    public FileTooBigException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to upload a file (picture), but already maximum number of them was uploaded
/// </summary>
public class FileLimitExeption : MinispaceException
{
    public FileLimitExeption() : base("Limit of files exceeded") { }
    public FileLimitExeption(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to upload a file (picture), that cannot be converted to required format
/// </summary>
public class FileFormatException : MinispaceException
{
    public FileFormatException() : base("File is not in valid format") { }
    public FileFormatException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to delete a file (picture), that does not exist
/// </summary>
public class FileIndexException : MinispaceException
{
    public FileIndexException() : base("File with this index does not exist") { }
    public FileIndexException(string message) : base(message) { }
}

/// <summary>
/// Thrown when operation failed for storage-related reasons
/// </summary>
public class StorageException : MinispaceException
{
    public StorageException() : base("File storage failed to fulfill the request") { }
    public StorageException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to send friend request to invalid target (f.e. yourself)
/// </summary>
public class FriendTargetException : MinispaceException
{
    public FriendTargetException() : base("This user cannot be befriended") { }
    public FriendTargetException(string message) : base(message) { }
}

/// <summary>
/// Thrown when given rating is not in range 0-5
/// </summary>
public class InvalidRatingValueException : MinispaceException
{
    public InvalidRatingValueException() : base("Invalid value for rating") { }
    public InvalidRatingValueException(string message) : base(message) { }
}
