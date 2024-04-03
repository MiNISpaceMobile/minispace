using Domain.BaseTypes;

namespace Domain.DataModel;

public class Comment : BaseEntity
{
    public Student? Author { get; set; }
    public Post Post { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public ICollection<Student> Likers { get; set; }

    public Comment? InResponseTo { get; set; }
    public ICollection<Comment> Responses { get; set; }

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
