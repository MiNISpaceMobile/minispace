namespace Api.DTO;

public record PostDto(long Id, Guid Uuid, long EventId, long AuthorId,
                      DateTime DatePosted, string Content);

public record RegisterPost(long EventId, string Content, DateTime DatePosted);
