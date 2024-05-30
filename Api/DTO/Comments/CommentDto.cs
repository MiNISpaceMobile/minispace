using Api.DTO.Users;

namespace Api.DTO.Comments;

public record CommentDto(
    Guid Guid,
    PublicUserDto? Author,
    DateTime CreationDate,
    string Content,
    int ResponsesCount);
