using Domain.BaseTypes;

namespace Domain.DataModel;

public enum ReportCategory
{
    Unknown, // For errors
    Behaviour,
    Bug,
    // More if neccessary
}

public enum ReportState
{
    Unknown, // For errors
    Waiting, // No Admin has seen it yet (no Responder, no Feedback)
    Rejected, // An Admin decided it should not be processed further (Responder and Feedback)
    Accepted, // An Admin is now working on it (Responder, but no Feedback)
    Success, // An Admin has successfully dealt with it (Responder and Feedback)
    Failure, // An Admin has failed to deal with it (Responder and Feedback)
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
    public ReportCategory Category { get; set; }
    public string? Feedback { get; set; }

    public ReportState State { get; set; }
    public bool IsOpen => State == ReportState.Waiting || State == ReportState.Accepted;
    public bool IsResolved => State == ReportState.Rejected || State == ReportState.Success;

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Report() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Report(User author, string title, string details, ReportCategory category)
    {
        Author = author;

        Title = title;
        Details = details;
        Category = category;

        State = ReportState.Waiting;
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

    public EventReport(Event reportedEvent, User author, string title, string details, ReportCategory category)
        : base(author, title, details, category)
    {
        ReportedEvent = reportedEvent;
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

    public PostReport(Post reportedPost, User author, string title, string details, ReportCategory category)
        : base(author, title, details, category)
    {
        ReportedPost = reportedPost;
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

    public CommentReport(Comment reportedComment, User author, string title, string details, ReportCategory category)
        : base(author, title, details, category)
    {
        ReportedComment = reportedComment;
    }
}
