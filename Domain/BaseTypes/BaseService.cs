using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services;

namespace Domain.BaseTypes;

public abstract class BaseService<ServiceType> where ServiceType : notnull, BaseService<ServiceType>
{
    public User? ActingUser { get; private set; }

    protected IUnitOfWork uow;

    protected BaseService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public ServiceType AsUser(Guid? guid)
    {
        if (guid.HasValue)
            ActingUser = uow.Repository<User>().GetOrThrow(guid.Value);
        else
            ActingUser = null;
        return (ServiceType)this;
    }

    protected void AllowOnlyAdmins()
    {
        if (ActingUser is not Administrator)
            throw new UserUnauthorizedException("Not an admin");
    }
    protected void AllowOnlyOrganizers(bool allowAdmins = true)
    {
        bool allowed = (allowAdmins && ActingUser is Administrator) || ((ActingUser as Student)?.IsOrganizer ?? false);
        if (!allowed)
            throw new UserUnauthorizedException("Not an organizer");
    }
    protected void AllowLoggedInStudents(bool allowAdmins = true)
    {
        bool allowed = (allowAdmins && ActingUser is Administrator) || ActingUser is Student;
        if (!allowed)
            throw new UserUnauthorizedException("Not logged in");
    }
    protected void AllowNotLoggedIn(bool allowAdmins = true)
    {
        bool allowed = (allowAdmins && ActingUser is Administrator) || ActingUser is null;
        if (!allowed)
            throw new UserUnauthorizedException("Logged in");
    }
    protected void AllowOnlyStudent(Student authorized, bool allowAdmins = true)
    {
        bool allowed = (allowAdmins && ActingUser is Administrator) || ActingUser == authorized;
        if (!allowed)
            throw new UserUnauthorizedException();
    }
    // Empty function, checks nothing, but is explicit about it
    protected void AllowEveryone() { }
}
