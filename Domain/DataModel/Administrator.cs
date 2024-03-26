using Domain.BaseTypes;

namespace Domain.DataModel;

public class Administrator : User
{
#pragma warning disable CS8618 // Unassigned non-nullables
    protected Administrator() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public Administrator(string username, string email, string password, DateTime? creationDate = null)
        : base(username, email, password, creationDate) { }  
}
