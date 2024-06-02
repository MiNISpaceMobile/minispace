using Domain.BaseTypes;

namespace Domain.DataModel;

public class User : BaseEntity
{
    // USOS is the source of ExternalId in our case
    public string? ExternalId { get; private set; }
    public DateTime CreationDate { get; private set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateTime DateOfBirth { get; set; }
    public int Age
    {
        get
        {
            int age = DateTime.Now.Year - DateOfBirth.Year;
            if (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear)
                age--;
            return age;
        }
    }

    public bool IsAdmin { get; set; }
    public bool IsOrganizer { get; set; }


    /* When adding A as B's friend you also need to add B as A's friend!
     * It does NOT happened automatically!
     */
    public virtual ICollection<User> Friends { get; }

    public virtual ICollection<Event> OrganizedEvents { get; }
    public virtual ICollection<Event> SubscribedEvents { get; }
    public virtual ICollection<Event> JoinedEvents { get; }

    public bool EmailNotification { get; set; }
    public IEnumerable<BaseNotification> AllNotifications
        => Enumerable.Concat<BaseNotification>(PersonalNotifications, SocialNotifications).Concat(ReceivedFriendRequests);
    public virtual ICollection<Notification> PersonalNotifications { get; }
    public virtual ICollection<SocialNotification> SocialNotifications { get; }
    public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; }
    public virtual ICollection<FriendRequest> SentFriendRequests { get; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected User() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public User(string firstName, string lastName, string email, DateTime dob, string? externalId = null, DateTime? creationDate = null)
    {
        ExternalId = externalId;
        CreationDate = creationDate ?? DateTime.Now;

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Description = "";

        DateOfBirth = dob;

        IsAdmin = false;
        IsOrganizer = false;

        Friends = new List<User>();

        OrganizedEvents = new List<Event>();
        SubscribedEvents = new List<Event>();
        JoinedEvents = new List<Event>();

        EmailNotification = true;

        PersonalNotifications = new List<Notification>();
        SocialNotifications = new List<SocialNotification>();
        ReceivedFriendRequests = new List<FriendRequest>();
        SentFriendRequests = new List<FriendRequest>();
    }
}
