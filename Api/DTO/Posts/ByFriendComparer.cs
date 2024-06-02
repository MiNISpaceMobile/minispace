namespace Api.DTO.Posts;

public class ByFriendComparer : IComparer<ReactionDto>
{
    public static readonly ByFriendComparer Instance = new ByFriendComparer();

    private ByFriendComparer() { }

    public int Compare(ReactionDto? x, ReactionDto? y)
    {
        if (x!.ByFriend == y!.ByFriend)
            return 0;
        return x.ByFriend ? -1 : 1;
    }
}
