using Api.DTO.Users;

namespace Api.DTO.Comments;

public record CommentDto(
    Guid Guid,
    UserDto? Author,
    string Content);
