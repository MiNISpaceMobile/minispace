using System.Text.Json.Serialization;

namespace Api.DTO;

public class Paging
{
    const int MinLimit = 1;
    const int DefLimit = 10;
    const int MaxLimit = 100;

    public int Limit { get; set; }
    public int Start { get; set; }
    public int? Size { get; set; }
    public int? End => Size.HasValue ? Start + Size.Value : null;

    public bool Ascending { get; set; }

    [JsonConstructor]
    public Paging(int? limit, int? start, int? size, bool? ascending)
    {
        Limit = Math.Clamp(limit ?? DefLimit, MinLimit, MaxLimit);
        Start = start ?? 0;
        Size = null; // size is for response only
        Ascending = ascending ?? true;
    }
}

public class Paged<Type>
{
    public IEnumerable<Type> Results { get; }
    public Paging Paging { get; }

    public Paged(IEnumerable<Type> results, Paging paging)
    {
        Results = results;
        Paging = paging;
    }

    public static Paged<Type> PageFrom<SortableType>(IEnumerable<Type> items, Func<Type, SortableType> sortablePropertySelector, Paging paging)
    {
        var results = (paging.Ascending ? items.OrderBy(sortablePropertySelector) : items.OrderByDescending(sortablePropertySelector))
                .Skip(paging.Start).Take(paging.Limit);
        paging.Size = results.Count();
        return new Paged<Type>(results, paging);
    }
}
