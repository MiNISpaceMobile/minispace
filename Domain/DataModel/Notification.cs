using Domain.BaseTypes;

namespace Domain.DataModel;

public enum NotificationType
{
    Unknown,
    EventNewPost, // When a Post is added to a subscribed Event 
    EventStartsSoon, // When a joined Event's StartDate is closer than 3 days
    CommentReponsedTo, // When someone responds to Student's Comment
}

public enum SocialNotificationType
{
    Unknown,
    FriendCommented, // When a Friend creates a Comment on a subscribed Event
    FriendJoinedEvent, // When a Friend joins an Event
    FriendOrganizesEvent, // When a Friend creates an Event
}

public abstract class BaseNotification : BaseEntity
{
    public Guid TargetId { get; private set; }
    public virtual Student Target { get; set; }

    public virtual Guid SourceId { get; protected set; }

    public bool Seen { get; set; }
    public DateTime Timestamp { get; private set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected BaseNotification() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public BaseNotification(Student target, DateTime? timestamp = null)
    {
        Target = target;

        Seen = false;
        Timestamp = timestamp ?? DateTime.Now;
    }
}

public class Notification : BaseNotification
{
    public NotificationType Type { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Notification() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Notification(Student target, BaseEntity source, NotificationType type, DateTime? timestamp = null)
        : base(target, timestamp)
    {
        SourceId = source.Guid;

        Type = type;
    }
}

public class SocialNotification : BaseNotification
{
    public Guid FriendId { get; private set; }
    public virtual Student Friend { get; set; }

    public SocialNotificationType Type { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected SocialNotification() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public SocialNotification(Student target, Student friend, BaseEntity source, SocialNotificationType type, DateTime? timestamp = null)
        : base(target, timestamp)
    {
        Friend = friend;

        SourceId = source.Guid;
        
        Type = type;
    }
}

public class FriendRequest : BaseNotification
{
    public Guid AuthorId { get; private set; }
    public virtual Student Author { get; set; }

    public override Guid SourceId => AuthorId;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected FriendRequest() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public FriendRequest(Student target, Student author, DateTime? timestamp = null)
        : base(target, timestamp)
    {
        Author = author;
    }
}

