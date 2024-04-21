namespace Api.DTO.Posts;

public record PostDto(
    long Id,
    Guid Uuid,
    long EventId,
    long AuthorId,
    DateTime DatePosted,
    string Content);
