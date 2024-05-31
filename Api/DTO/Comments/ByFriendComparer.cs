namespace Api.DTO.Comments;

public class ByFriendComparer : IComparer<LikeDto>
{
    public static readonly ByFriendComparer Instance = new ByFriendComparer();

    private ByFriendComparer() { }

    public int Compare(LikeDto? x, LikeDto? y)
    {
        if (x!.ByFriend == y!.ByFriend)
            return 0;
        return x.ByFriend ? -1 : 1;
    }
}
