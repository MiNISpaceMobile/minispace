namespace Api.DTO.Users;

public record UserDto(
    Guid Guid,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    DateTime DateOfBirth,
    bool IsAdmin,
    bool IsOrganizer,
    bool EmailNotifications);
