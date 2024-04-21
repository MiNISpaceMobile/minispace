namespace Api.DTO;

public enum UserRole
{
    ORGANIZER,
    ADMIN
}

public record UserDto(
    long Id, 
    string FirstName, 
    string LastName, 
    long StudentNumber,
    IEnumerable<string> Roles, 
    long UsosId);

public record UserSearchDetails(
    string Name, 
    long StudentNumber, 
    PageableRequest Pageable);