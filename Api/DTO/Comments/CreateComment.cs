namespace Api.DTO.Comments;

public record CreateComment(
    Guid AuthorGuid,
    Guid PostGuid,
    string Content,
    Guid? InResponseTo);