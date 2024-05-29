using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO.Posts;

public record PostDto(
    Guid Guid,
    string Content,
    Guid EventGuid,
    string EventTitle,
    PublicUserDto? Author,
    DateTime CreationDate,
    ReactionType? UserReaction,
    int ReactionCount,
    IEnumerable<string> PictureUrls);
