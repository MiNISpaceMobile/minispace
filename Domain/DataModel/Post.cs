using Domain.BaseTypes;

namespace Domain.DataModel;

public enum ReactionType
{
    Unknown,
    Like,
    Funny,
    Wow,
    Angry,
    Sad,
}

public class Reaction
{
    public Guid AuthorId { get; private set; }
    public virtual User Author { get; set; }

    public Guid PostId { get; protected set; }
    public virtual Post Post { get; set; }

    public ReactionType Type { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Reaction() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Reaction(User author, Post post, ReactionType type)
    {
        Author = author;

        Post = post;

        Type = type;
    }
}

public class Post : BaseEntity
{
    public Guid? AuthorId { get; private set; }
    public virtual User? Author { get; set; }
    public Guid EventId { get; private set; }
    public virtual Event Event { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Comment> Comments { get; }
    public virtual ICollection<PostPicture> Pictures { get; }
    public virtual ICollection<Reaction> Reactions { get; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Post() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Post(User author, Event @event, string content, DateTime? creationDate = null)
    {
        Author = author;

        Event = @event;

        Content = content;

        CreationDate = creationDate ?? DateTime.Now;
        
        Comments = new List<Comment>();
        Pictures = new List<PostPicture>();
        Reactions = new List<Reaction>();
    }
}
