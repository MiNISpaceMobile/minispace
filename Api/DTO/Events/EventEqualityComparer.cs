using Domain.DataModel;

namespace Api.DTO.Events;

public class EventEqualityComparer : EqualityComparer<Event>
{
    private IEqualityComparer<Guid> _c = EqualityComparer<Guid>.Default;

    public override bool Equals(Event? l, Event? r)
    {
        return _c.Equals(l!.Guid, r!.Guid);
    }

    public override int GetHashCode(Event rule)
    {
        return _c.GetHashCode(rule.Guid);
    }
}

