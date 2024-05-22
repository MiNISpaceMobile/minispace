namespace Domain.DataModel;

public abstract class Picture
{
    public int Id { get; set; }

    public int Index { get; set; }
    public string Url { get; set; }

    public abstract Guid SourceId { get; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Picture() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Picture(int index, string url)
    {
        Index = index;
        Url = url;
    }
}

public class EventPicture : Picture
{
    public Guid EventId { get; private set; }
    public virtual Event Event { get; set; }

    public override Guid SourceId => EventId;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected EventPicture() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public EventPicture(Event @event, int index, string url) : base(index, url)
    {
        Event = @event;
    }
}

public class PostPicture : Picture
{
    public Guid PostId { get; private set; }
    public virtual Post Post { get; set; }

    public override Guid SourceId => PostId;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected PostPicture() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public PostPicture(Post post, int index, string url) : base(index, url)
    {
        Post = post;
    }
}
