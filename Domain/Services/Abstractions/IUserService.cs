using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IUserService : IBaseService<IUserService>
{
    public IEnumerable<User> GetUsers();

    public User GetUser(Guid guid);

    public User CreateUser(string firstName, string lastName, string email, DateTime dob, string? externalId = null);

    public void UpdateUser(User newStudent);

    public void DeleteUser();

    /* Returns potentialy modified roles of target User
     * If role parameter is not null sets corresponding role to its value
     */
    public (bool IsAdmin, bool IsOrganizer) UserRoles(Guid target, bool? isAdmin, bool? isOrganizer);

    /* Returns newly created FriendRequest
     * unless an opposite FriendRequest was already pending
     * in which case it is accepted, deleted and function returns null
     */
    public FriendRequest? SendFriendRequest(Guid targetId);

    public void RespondFriendRequest(Guid requestId, bool accept);
}
