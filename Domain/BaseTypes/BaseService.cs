using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services;

namespace Domain.BaseTypes;

public interface IBaseService<ServiceType> where ServiceType : notnull, IBaseService<ServiceType>
{
    public User? ActingUser { get; }
    public ServiceType AsUser(Guid? guid);
}

public abstract class BaseService<ServiceInterface, ServiceImplementaion> : IBaseService<ServiceInterface>
    where ServiceInterface : notnull, IBaseService<ServiceInterface>
    where ServiceImplementaion : notnull, BaseService<ServiceInterface, ServiceImplementaion>, ServiceInterface
{
    public User? ActingUser { get; private set; }

    protected IUnitOfWork uow;

    protected BaseService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public ServiceInterface AsUser(Guid? guid)
    {
        if (guid.HasValue)
            ActingUser = uow.Repository<User>().GetOrThrow(guid.Value);
        else
            ActingUser = null;
        return (ServiceImplementaion)this;
    }

    protected void AllowOnlyAdmins()
    {
        if (ActingUser is Administrator)
            return;
        throw new UserUnauthorizedException("Not an admin");
    }
    protected void AllowOnlyOrganizers()
    {
        if ((ActingUser as Student)?.IsOrganizer ?? false)
            return;
        throw new UserUnauthorizedException("Not an organizer");
    }
    protected void AllowOnlyStudents()
    {
        if (ActingUser is Student)
            return;
        throw new UserUnauthorizedException("Not a student");
    }
    protected void AllowAllUsers()
    {
        if (ActingUser is not null)
            return;
        throw new UserUnauthorizedException("Not logged in");
    }
    protected void AllowOnlyNotLoggedIn(bool allowAdmins = true)
    {
        if ((allowAdmins && ActingUser is Administrator) || ActingUser is null)
            return;
        throw new UserUnauthorizedException("Logged in");
    }
    // If passed in User is null, then NO non-admin is authorized
    // Passing in (null, false) will result in Exception no matter the ActingUser
    protected void AllowOnlyUser(User? authorized, bool allowAdmins = true)
    {
        if ((allowAdmins && ActingUser is Administrator) || (authorized is not null && Equals(ActingUser, authorized)))
            return;
        throw new UserUnauthorizedException();
    }
    // Empty function, which checks nothing, but is explicit about it
    protected void AllowEveryone() { }
}
