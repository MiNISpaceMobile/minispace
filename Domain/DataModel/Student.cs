namespace Domain.DataModel;

public class Student : User
{
    /* When adding A as B's friend you also need to add B as A's friend!
     * It does NOT happened automatically!
     */
    public virtual ICollection<Student> Friends { get; }

    public virtual ICollection<Event> SubscribedEvents { get; }

    public string Description { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age
    {
        get
        {
            if (DateOfBirth is null)
                return null;

            int age = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (DateTime.Now.DayOfYear < DateOfBirth.Value.DayOfYear)
                age--;
            return age;
        }
    }

    public bool EmailNotification { get; set; }

    public bool IsOrganizer { get; set; }
    public virtual ICollection<Event> OrganizedEvents { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    protected Student() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Student(string firstName, string lastName, string email, string? externalId = null, DateTime? creationDate = null)
        : base(firstName, lastName, email, externalId, creationDate)
    {
        Friends = new List<Student>();
        SubscribedEvents = new List<Event>();

        Description = "";

        EmailNotification = true;

        IsOrganizer = false;
        OrganizedEvents = new List<Event>();
    }  
}
