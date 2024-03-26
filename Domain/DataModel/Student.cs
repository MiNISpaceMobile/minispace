using Domain.BaseTypes;

namespace Domain.DataModel;

public class Student : User
{
    public List<Student> Friends { get; }
    // public List<Event> SubscribedEvents { get; }

    public string Description { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public bool EmailNotification { get; set; }

    public bool IsOrganizer { get; set; }

    protected Student() { }

    public Student(string username, string email, string password, DateTime? creationDate = null)
        : base(username, email, password, creationDate)
    {
        Friends = new List<Student>();
        // SubscribedEvents = new List<Event>();

        Description = "";

        EmailNotification = true;

        IsOrganizer = false;
    }  
}
