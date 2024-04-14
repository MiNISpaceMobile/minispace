using Domain.BaseTypes;

namespace Domain.DataModel;

public enum NotificationType
{
    Unknown,
    EventNewPost, // When a Post is added to a subscribed Event 
    EventStartsSoon, // When a joined Event's StartDate is closer than 3 days
    CommentReponse, // When someone responds to Student's Comment
    FriendCommented, // When a Friend creates a Comment on a subscribed Event
    FriendJoinedEvent, // When a Friend joins an Event
    FriendOrganizesEvent, // When a Friend creates an Event
    ReceivedFriendInvite, // When someone sends Student a FriendInvite
}

public class Notification : BaseEntity
{
    public Guid TargetId { get; private set; }
    public virtual Student Target { get; set; }

    public Guid? FriendId { get; private set; }
    public virtual Student? Friend { get; set; }

    public Guid SourceId { get; }
    public NotificationType Type { get; private set; }

    public bool Seen { get; set; }
    public DateTime Timestamp { get; private set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Notification() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Notification(Student target, Student? friend, Guid sourceId, NotificationType type, DateTime? timestamp = null)
    {
        Target = target;
        Friend = friend;

        SourceId = sourceId;
        Type = type;

        Seen = false;
        Timestamp = timestamp ?? DateTime.Now;
    }
}
