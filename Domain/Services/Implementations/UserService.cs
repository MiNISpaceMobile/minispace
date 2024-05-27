using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class UserService(IUnitOfWork uow, IStorage storage)
    : BaseService<IUserService, UserService>(uow), IUserService
{
    public IEnumerable<User> GetUsers()
    {
        AllowOnlyAdmins();

        return uow.Repository<User>().GetAll();
    }

    public User GetUser(Guid guid)
    {
        AllowOnlyAdmins();

        User student = uow.Repository<User>().GetOrThrow(guid);

        return student;
    }

    public User GetUser()
    {
        AllowOnlyLoggedIn();

        return ActingUser!;
    }

    public User CreateUser(string firstName, string lastName, string email, DateTime dob, string? externalId = null)
    {
        AllowOnlyNotLoggedIn();

        User student = new User(firstName, lastName, email, dob, externalId);

        uow.Repository<User>().Add(student);
        uow.Commit();

        return student;
    }

    public User UpdateUser(string? firstName, string? lastName, string? email, string? description, DateTime? dob, bool? emailNotification)
    {
        AllowOnlyLoggedIn();

        User student = ActingUser!;

        if (firstName is not null)
            student.FirstName = firstName;
        if (lastName is not null)
            student.LastName = lastName;
        if (email is not null)
            student.Email = email;
        if (description is not null)
            student.Description = description;
        if (dob.HasValue)
            student.DateOfBirth = dob.Value;
        if (emailNotification.HasValue)
            student.EmailNotification = emailNotification.Value;

        uow.Commit();

        return student;
    }

    public void DeleteUser()
    {
        AllowOnlyLoggedIn();

        uow.Repository<User>().Delete(ActingUser!);
        storage.TryDeleteDirectory(IStorage.UserDirectory(ActingUser!.Guid));
        uow.Commit();
    }

    public (bool IsAdmin, bool IsOrganizer) UserRoles(Guid targetGuid, bool? isAdmin, bool? isOrganizer)
    {
        AllowOnlyAdmins();

        User user = uow.Repository<User>().GetOrThrow(targetGuid);
        if (isAdmin.HasValue || isOrganizer.HasValue)
        {
            if (isAdmin.HasValue)
                user.IsAdmin = isAdmin.Value;
            if (isOrganizer.HasValue)
                user.IsOrganizer = isOrganizer.Value;
            uow.Commit();
        }
        return (user.IsAdmin, user.IsOrganizer);
    }

    public FriendRequest? SendFriendRequest(Guid targetUserId)
    {
        AllowOnlyLoggedIn();

        User author = ActingUser!;

        if (author.Guid == targetUserId)
            throw new FriendTargetException("Can't befriend self");

        User target = uow.Repository<User>().GetOrThrow(targetUserId);

        FriendRequest? opposite = target.SentFriendRequests.Where(r => r.Target == author).SingleOrDefault();
        if (opposite is not null) // Author already received a FriendRequest from target
        {
            target.Friends.Add(author);
            author.Friends.Add(target);

            uow.Repository<FriendRequest>().Delete(opposite);
            uow.Commit();

            return null;
        }

        if (target.Friends.Contains(author))
            throw new FriendTargetException("Already friends");

        if (target.ReceivedFriendRequests.Any(r => r.Author == author))
            throw new FriendTargetException("Already sent a friend request");

        FriendRequest request = new FriendRequest(target, author);
        uow.Repository<FriendRequest>().Add(request);
        uow.Commit();

        return request;
    }

    public void RespondFriendRequest(Guid requestId, bool accept)
    {
        FriendRequest request = uow.Repository<FriendRequest>().GetOrThrow(requestId);

        AllowOnlyUser(request.Target);

        if (accept)
        {
            User target = request.Target;
            User author = request.Author;

            target.Friends.Add(author);
            author.Friends.Add(target);
        }

        uow.Repository<FriendRequest>().Delete(request);
        uow.Commit();
    }

    public void CancelFriendRequest(Guid requestId)
    {
        FriendRequest request = uow.Repository<FriendRequest>().GetOrThrow(requestId);

        AllowOnlyUser(request.Target);

        uow.Repository<FriendRequest>().Delete(request);
        uow.Commit();
    }

    public IEnumerable<BaseNotification> GetNotifications()
    {
        AllowOnlyLoggedIn();

        return ActingUser!.AllNotifications;
    }

    public void SeeAllNotifications()
    {
        AllowOnlyLoggedIn();

        foreach (var notification in ActingUser!.AllNotifications)
            notification.Seen = true;
        uow.Commit();
    }

    public void SeeNotification(Guid guid)
    {
        BaseNotification notification = uow.Repository<BaseNotification>().GetOrThrow(guid);

        AllowOnlyUser(notification.Target);

        notification.Seen = true;
        uow.Commit();
    }
}
