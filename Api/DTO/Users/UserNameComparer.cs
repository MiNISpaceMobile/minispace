namespace Api.DTO.Users;

public class UserNameComparer : IComparer<PublicUserDto>
{
    public static readonly UserNameComparer Instance = new();

    private UserNameComparer() { }

    public int Compare(PublicUserDto? x, PublicUserDto? y)
    {
        int compareLastName = x!.LastName.CompareTo(y!.LastName);
        return compareLastName != 0 ? compareLastName : x.FirstName.CompareTo(y.FirstName);
    }
}
