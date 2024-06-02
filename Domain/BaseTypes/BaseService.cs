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
        AllowOnlyLoggedIn();

        if (ActingUser!.IsAdmin)
            return;

        throw new UserUnauthorizedException("Acting user is not an admin");
    }
    protected void AllowOnlyOrganizers(bool alsoAllowAdmins = true)
    {
        AllowOnlyLoggedIn();

        if (ActingUser!.IsOrganizer)
            return;
        if (alsoAllowAdmins && ActingUser.IsAdmin)
            return;

        throw new UserUnauthorizedException("Acting user is not an organizer");
    }
    protected void AllowOnlyLoggedIn()
    {
        if (ActingUser is not null)
            return;

        throw new UserUnauthorizedException("Acting user is not logged in");
    }
    protected void AllowOnlyNotLoggedIn(bool alsoAllowAdmins = true)
    {
        if (ActingUser is null)
            return;
        if (alsoAllowAdmins && ActingUser.IsAdmin)
            return;

        throw new UserUnauthorizedException("This action is only for non-logged in users");
    }
    // If passed in User is null, then NO non-admin is authorized
    // Passing in (null, false) will result in Exception no matter the ActingUser
    protected void AllowOnlyUser(User? authorized, bool alsoAllowAdmins = true)
    {
        AllowOnlyLoggedIn();

        if (Equals(ActingUser, authorized))
            return;
        if (alsoAllowAdmins && ActingUser!.IsAdmin)
            return;

        throw new UserUnauthorizedException();
    }
    // Empty function, which checks nothing, but is explicit about it
    protected void AllowEveryone() { }
}
