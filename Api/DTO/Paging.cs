using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Api.DTO;

public class Paging
{
    public const int MinLimit = 1;
    public const int DefLimit = 10;
    public const int MaxLimit = 100;

    private int limit;
    public int Limit
    {
        get => limit;
        set => limit = Math.Clamp(value, MinLimit, MaxLimit);
    }
    public int Start { get; set; }
    [BindNever]
    public int? Size { get; set; }
    [BindNever]
    public int? End => Size.HasValue ? Start + Size.Value : null;
    [BindNever]
    public bool? Last { get; set; }

    public Paging() : this(null, null, null, null) { }

    [JsonConstructor]
    public Paging(int? limit, int? start, int? size, bool? last)
    {
        Limit = limit ?? DefLimit;
        Start = start ?? 0;
        Size = null; // size is for response only
        Last = null; // last is for response only
    }
}

public class Paged<Type>
{
    public IEnumerable<Type> Results { get; }
    public Paging Paging { get; }

    private Paged(IEnumerable<Type> results, Paging paging)
    {
        Results = results;
        Paging = paging;
    }

    public static Paged<Type> PageFrom(IEnumerable<Type> items, IComparer<Type> comparer, Paging paging)
    {
        paging.Last = items.Count() <= paging.Start + paging.Limit;
        var results = items.OrderBy(x => x, comparer).Skip(paging.Start).Take(paging.Limit);
        paging.Size = results.Count();
        return new Paged<Type>(results, paging);
    }
}
