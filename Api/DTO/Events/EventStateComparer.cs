namespace Api.DTO.Events;

public class EventStateComparer : IComparer<ListEventDto>
{
    public static readonly EventStateComparer Instance = new();

    private EventStateComparer() { }

    /// <summary>
    /// returns most recent events prioritizing not ended
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(ListEventDto x, ListEventDto y)
    {
        if (x.EndDate > DateTime.Now && y.EndDate > DateTime.Now)
            return x.StartDate.CompareTo(y.StartDate);
        else if (x.EndDate > DateTime.Now)
            return -1;
        else if (y.EndDate > DateTime.Now)
            return 1;
        else
            return -x.EndDate.CompareTo(y.EndDate);
    }
}
