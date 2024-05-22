namespace Api.DTO.Users;

public record UserDto(
    Guid Guid,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePictureUrl);
