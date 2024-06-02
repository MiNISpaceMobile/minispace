using Api.DTO.Users;

namespace Api.DTO.Posts;

public record ListPostDto(
    Guid Guid,
    string Title,
    string Content,
    IEnumerable<string> PictureUrls);
