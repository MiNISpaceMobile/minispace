namespace Api.DTO.Users;

public record UpdateUserDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Description,
    DateTime? DateOfBirth,
    bool? EmailNotifications);
