using Api.DTO.Users;

namespace Api.DTO.Comments;

public record CommentDto(
    Guid Guid,
    PublicUserDto? Author,
    bool? UserReactionIsDislike,
    int LikeCount,
    DateTime CreationDate,
    string Content,
    int ResponsesCount);
