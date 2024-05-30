using Domain.BaseTypes;

namespace Domain.DataModel;

public class Post : BaseEntity
{
    public Guid? AuthorId { get; private set; }
    public virtual User? Author { get; set; }
    public Guid EventId { get; private set; }
    public virtual Event Event { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Comment> Comments { get; }
    public virtual ICollection<PostPicture> Pictures { get; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Post() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Post(User author, Event @event, string title, string content, DateTime? creationDate = null)
    {
        Author = author;

        Event = @event;

        Title = title;
        Content = content;

        CreationDate = creationDate ?? DateTime.Now;
        
        Comments = new List<Comment>();
        Pictures = new List<PostPicture>();
    }
}
