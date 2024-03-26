using Domain.BaseTypes;

namespace Domain.DataModel;

public class Administrator : User
{
    protected Administrator() { }

    public Administrator(string username, string email, string password, DateTime? creationDate = null)
        : base(username, email, password, creationDate) { }  
}
