﻿using Domain.BaseTypes;

namespace Domain.DataModel;

public class Post : BaseEntity
{
    public Guid? AuthorId { get; private set; }
    public virtual Student? Author { get; set; }
    public Guid EventId { get; private set; }
    public virtual Event Event { get; set; }

    public string Content { get; set; }
    // public ICollection<string> Pictures { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Post() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Post(Student author, string content, DateTime? creationDate = null)
    {
        Author = author;

        Content = content;

        CreationDate = creationDate ?? DateTime.Now;

        Comments = new List<Comment>();
    }
}