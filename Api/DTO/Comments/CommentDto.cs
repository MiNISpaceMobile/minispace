using Api.DTO.Users;

namespace Api.DTO.Comments;

public record CommentDto(
    long EventId,
    long PostId,
    UserDto? Author,
    string Content);
