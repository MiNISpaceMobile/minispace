namespace Api.DTO;

public record CommentDto(
    long EventId, 
    long PostId, 
    UserDto? Author, 
    string Content);

// APISpec says that only one id should be present
public record CommentSearchDetails(
    long? EventId,
    long? PostId,
    PageableRequest Pageable);

// APISpec says that only one id should be present
public record CreateComment(
    long? EventId,
    long? PostId,
    string Content);