namespace Api.DTO.Users;

public record PublicUserDto(
    Guid Guid,
    string FirstName,
    string LastName,
    string Description,
    string? ProfilePictureUrl = null);
