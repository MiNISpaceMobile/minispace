namespace Api.DTO.Posts;

public record CreatePost(
    Guid AuthorGuid,
    Guid EventGuid,
    string Content);