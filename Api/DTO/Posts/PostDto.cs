using Api.DTO.Users;

namespace Api.DTO.Posts;

public record PostDto(
    Guid Guid,
    Guid EventGuid,
    UserDto? Author,
    DateTime CreationDate,
    IEnumerable<string> PictureUrls);
