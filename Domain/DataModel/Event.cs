﻿using Domain.BaseTypes;

namespace Domain.DataModel;

public enum EventCategory
{
    Unknown, // For errors
    Uncategorized,
    // Any more we think of
}

public class Feedback
{
    public Guid AuthorId { get; private set; }
    public virtual User Author { get; set; }

    public Guid EventId { get; private set; }
    public virtual Event Event { get; set; }

    public int Rating { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Feedback() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Feedback(User author, Event @event, int rating)
    {
        Author = author;
        Event = @event;

        Rating = rating;
    }
}

public class Event : BaseEntity
{
    public Guid? OrganizerId { get; private set; }
    public virtual User? Organizer { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public EventCategory Category { get; set; }

    public DateTime PublicationDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }

    public int? Capacity { get; set; }
    public decimal? Fee { get; set; }

    public virtual ICollection<User> Interested { get; }
    public virtual ICollection<User> Participants { get; }

    public virtual ICollection<Post> Posts { get; }
    public virtual ICollection<Feedback> Feedback { get; }
    public virtual ICollection<EventPicture> Pictures { get; }

    public int ViewCount { get; set; }
    public int? AverageAge
    {
        get
        {
            var ages = Participants.Select(p => p.Age);
            return ages.Any() ? ages.Sum() / ages.Count() : null;
        }
    }

    public float? Rating =>
            Feedback.Count == 0 ? null : (float)Feedback.Average(x => x.Rating);


#pragma warning disable CS8618 // Unassigned non-nullables
    protected Event() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Event(User organizer, string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee)
    {
        Organizer = organizer;

        Title = title;
        Description = description;
        Category = category;

        PublicationDate = publicationDate;
        StartDate = startDate;
        EndDate = endDate;
        Location = location;

        Capacity = capacity;
        Fee = fee;

        Participants = new List<User>();
        Interested = new List<User>();

        Posts = new List<Post>();
        Feedback = new List<Feedback>();
        Pictures = new List<EventPicture>();

        ViewCount = 0;
    }
}
