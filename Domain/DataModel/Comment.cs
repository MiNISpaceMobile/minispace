using Domain.BaseTypes;

namespace Domain.DataModel;

public class Like
{
    public Guid AuthorId { get; private set; }
    public virtual User Author { get; set; }

    public Guid CommentId { get; protected set; }
    public virtual Comment Comment { get; set; }

    public bool IsDislike { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Like() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Like(User author, Comment comment, bool isDislike)
    {
        Author = author;

        Comment = comment;

        IsDislike = isDislike;
    }
}

public class Comment : BaseEntity
{
    public Guid? AuthorId { get; private set; }
    public virtual User? Author { get; set; }
    public Guid PostId { get; private set; }
    public virtual Post Post { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Like> Likes { get; set; }

    public Guid? InResponeseToId { get; private set; }
    public virtual Comment? InResponseTo { get; set; }
    public virtual ICollection<Comment> Responses { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Comment() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Comment(User author, Post post, string content, Comment? inResponseTo, DateTime? creationDate = null)
    {
        Author = author;
        Post = post;

        Content = content;

        CreationDate = creationDate ?? DateTime.Now;

        Likes = new List<Like>();

        InResponseTo = inResponseTo;
        Responses = new List<Comment>();
    }
}
