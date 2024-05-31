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
    ReactionsSummary Reactions,
    IEnumerable<string> PictureUrls);
