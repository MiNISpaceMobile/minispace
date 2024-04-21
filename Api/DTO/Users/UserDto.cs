namespace Api.DTO.Users;

public record UserDto(
    long Id,
    string FirstName,
    string LastName,
    long StudentNumber,
    IEnumerable<string> Roles,
    long UsosId);
