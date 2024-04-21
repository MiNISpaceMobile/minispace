namespace Api.DTO.Posts;

public record RegisterPost(
    long EventId,
    string Content,
    DateTime DatePosted);