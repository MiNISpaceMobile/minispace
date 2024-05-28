using Domain.BaseTypes;

namespace Domain.DataModel;

public enum ReportType
{
    Event,
    Post,
    Comment
}

public abstract class Report : BaseEntity
{
    public Guid? AuthorId { get; private set; }
    public virtual User? Author { get; set; }
    public Guid? ResponderId { get; private set; }
    public virtual User? Responder { get; set; }

    public abstract Guid TargetId { get; }

    public string Title { get; set; }
    public string Details { get; set; }
    public ReportType ReportType { get; protected set; }
    public string? Feedback { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool IsOpen { get; set; }


#pragma warning disable CS8618 // Unassigned non-nullables
    protected Report() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Report(User author, string title, string details)
    {
        Author = author;

        Title = title;
        Details = details;

        IsOpen = true;
        CreationDate = DateTime.Now;
        UpdateDate = CreationDate;
    }
}

public class EventReport : Report
{
    public Guid ReportedEventId { get; private set; }
    public virtual Event ReportedEvent { get; set; }

    public override Guid TargetId => ReportedEventId;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected EventReport() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public EventReport(Event reportedEvent, User author, string title, string details)
        : base(author, title, details)
    {
        ReportedEvent = reportedEvent;
        ReportType = ReportType.Event;
    }
}

public class PostReport : Report
{
    public Guid ReportedPostId { get; private set; }
    public virtual Post ReportedPost { get; set; }

    public override Guid TargetId => ReportedPostId;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected PostReport() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public PostReport(Post reportedPost, User author, string title, string details)
        : base(author, title, details)
    {
        ReportedPost = reportedPost;
        ReportType = ReportType.Post;
    }
}

public class CommentReport : Report
{
    public Guid ReportedCommentId { get; private set; }
    public virtual Comment ReportedComment { get; set; }

    public override Guid TargetId => ReportedCommentId;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected CommentReport() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public CommentReport(Comment reportedComment, User author, string title, string details)
        : base(author, title, details)
    {
        ReportedComment = reportedComment;
        ReportType = ReportType.Comment;
    }
}
