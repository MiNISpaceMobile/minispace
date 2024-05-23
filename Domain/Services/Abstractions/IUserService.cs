using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IUserService : IBaseService<IUserService>
{
    public IEnumerable<User> GetUsers();

    public User GetUser(Guid guid);

    public User GetUser();

    public User CreateUser(string firstName, string lastName, string email, DateTime dob, string? externalId = null);

    public User UpdateUser(string? firstName, string? lastName, string? email, string? description, DateTime? dob, bool? emailNotification);

    public void DeleteUser();

    /* Returns potentialy modified roles of target User
     * If role parameter is not null sets corresponding role to its value
     */
    public (bool IsAdmin, bool IsOrganizer) UserRoles(Guid target, bool? isAdmin, bool? isOrganizer);

    /* Returns newly created FriendRequest
     * unless an opposite FriendRequest was already pending
     * in which case it is accepted, deleted and function returns null
     */
    public FriendRequest? SendFriendRequest(Guid targetUserId);

    public void RespondFriendRequest(Guid requestId, bool accept);

    public void CancelFriendRequest(Guid requestId);

    public IEnumerable<BaseNotification> GetNotifications();

    public void SeeAllNotifications();

    public void SeeNotification(Guid guid);
}
