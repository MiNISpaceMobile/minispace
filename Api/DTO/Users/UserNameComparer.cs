namespace Api.DTO.Users;

public class UserNameComparer : IComparer<UserDto>
{
    public static readonly UserNameComparer Instance = new();

    private UserNameComparer() { }

    public int Compare(UserDto? x, UserDto? y)
    {
        int compareLastName = x!.LastName.CompareTo(y!.LastName);
        return compareLastName != 0 ? compareLastName : x.FirstName.CompareTo(y.FirstName);
    }
}
