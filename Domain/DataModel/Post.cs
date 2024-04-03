using Domain.BaseTypes;

namespace Domain.DataModel;

public class Post : BaseEntity
{
    public Student? Author { get; set; }
    public Event Event { get; set; }

    public string Content { get; set; }
    // public List<string> Pictures { get; set; }

    public DateTime CreationDate { get; set; }

    public List<Comment> Comments { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Post() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Post(Student author, Event @event, string content, DateTime? creationDate = null)
    {
        Author = author;
        Event = @event;

        Content = content;

        CreationDate = creationDate ?? DateTime.Now;

        Comments = new List<Comment>();
    }
}