using Domain.BaseTypes;

namespace Domain.DataModel;

public class Comment : BaseEntity
{
    public Guid? AuthorId { get; private set; }
    public virtual Student? Author { get; set; }
    public Guid PostId { get; private set; }
    public virtual Post Post { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Student> Likers { get; set; }

    public Guid? InResponeseToId { get; private set; }
    public virtual Comment? InResponseTo { get; set; }
    public virtual ICollection<Comment> Responses { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Comment() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Comment(Student author, Post post, string content, Comment? inResponseTo, DateTime? creationDate = null)
    {
        Author = author;
        Post = post;

        Content = content;

        CreationDate = creationDate ?? DateTime.Now;

        Likers = new List<Student>();

        InResponseTo = inResponseTo;
        Responses = new List<Comment>();
    }
}
