namespace Api.DTO.Comments;

// APISpec says that only one id should be present
public record CreateComment(
    long? EventId,
    long? PostId,
    string Content);