namespace Api.DTO.Comments;

public class CreationDateComparer : IComparer<CommentDto>
{
    public static readonly CreationDateComparer Instance = new();

    private CreationDateComparer() { }

    /// <summary>
    /// From newest to oldest
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(CommentDto? x, CommentDto? y)
    {
        return x!.CreationDate.CompareTo(y!.CreationDate);
    }
}
