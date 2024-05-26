using Api.DTO.Users;

namespace Api.DTO.Posts;

public record PostDto(
    Guid Guid,
    string Content,
    Guid EventGuid,
    string EventTitle,
    UserDto? Author,
    DateTime CreationDate,
    IEnumerable<string> PictureUrls);
