using Api.DTO.Users;

namespace Api.DTO.Comments;

public record LikeDto(PublicUserDto Author, bool IsDislike, bool ByFriend);
