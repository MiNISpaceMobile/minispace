namespace Api.DTO.Students;

public record StudentDto(
    Guid Guid,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    DateTime? DateOfBirth,
    int? Age,
    bool IsOrganizer);
