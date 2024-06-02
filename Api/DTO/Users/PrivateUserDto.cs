namespace Api.DTO.Users;

public record PrivateUserDto(
    Guid Guid,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    DateTime DateOfBirth,
    bool IsAdmin,
    bool IsOrganizer,
    bool EmailNotifications,
    string? ProfilePictureUrl
    ) : PublicUserDto(Guid, FirstName, LastName, Description, ProfilePictureUrl);
