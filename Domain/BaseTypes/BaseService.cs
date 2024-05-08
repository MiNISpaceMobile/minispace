﻿using Domain.Abstractions;
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

    // TODO: Actually use these guards in derived Services

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
    // Empty function, which checks nothing, but is explicit about it
    protected void AllowEveryone() { }
}
