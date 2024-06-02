namespace Api.DTO.Comments;

public record CreateComment(
    Guid PostGuid,
    string Content,
    Guid? InResponseTo);