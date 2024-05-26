namespace Api.DTO.Posts;

public class CreationDateComparer : IComparer<PostDto>
{
    public static readonly CreationDateComparer Instance = new();

    private CreationDateComparer() { }

    /// <summary>
    /// From newest to oldest
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(PostDto? x, PostDto? y)
    {
        return y!.CreationDate.CompareTo(x!.CreationDate);
    }
}
