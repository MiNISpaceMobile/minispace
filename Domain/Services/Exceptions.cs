using Domain.BaseTypes;

namespace Domain.Services;

/// <summary>
/// Thrown when trying to get nonexistent object from database
/// </summary>
public class InvalidGuidException : Exception
{
    public InvalidGuidException(string message) : base(message) { }
    public InvalidGuidException() : base($"No Entity with given Guid") { }
}
/// <summary>
/// Thrown when trying to get nonexistent object from database
/// </summary>
public class InvalidGuidException<RecordType> : InvalidGuidException where RecordType : notnull, BaseEntity
{
    public InvalidGuidException(string message) : base(message) { }
    public InvalidGuidException() : base($"No {typeof(RecordType).Name} with given Guid") { }
}

/// <summary>
/// Thrown when trying to use service method while unathorized
/// </summary>
public class UserUnauthorizedException : Exception
{
    public UserUnauthorizedException(string message) : base(message) { }
    public UserUnauthorizedException() : base("Acting user is not authorized to perform this action") { }
}

/// <summary>
/// Thrown when trying to assign empty content to an object
/// </summary>
public class EmptyContentException : Exception
{
    public EmptyContentException() : base("Content must not be empty") { }
    public EmptyContentException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to upload a file (picture), that is too big
/// </summary>
public class FileTooBigException : Exception
{
    public FileTooBigException() : base("File is too big") { }
    public FileTooBigException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to upload a file (picture), but already maximum number of them was uploaded
/// </summary>
public class FileLimitExeption : Exception
{
    public FileLimitExeption() : base("Limit of files exceeded") { }
    public FileLimitExeption(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to upload a file (picture), that cannot be converted to required format
/// </summary>
public class FileFormatException : Exception
{
    public FileFormatException() : base("File is not in valid format") { }
    public FileFormatException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to delete a file (picture), that does not exist
/// </summary>
public class FileIndexException : Exception
{
    public FileIndexException() : base("File with this index does not exist") { }
    public FileIndexException(string message) : base(message) { }
}

/// <summary>
/// Thrown when operation failed for storage-related reasons
/// </summary>
public class StorageException : Exception
{
    public StorageException() : base("File storage failed to fulfill the request") { }
    public StorageException(string message) : base(message) { }
}
