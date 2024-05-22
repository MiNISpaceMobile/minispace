using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class UserService(IUnitOfWork uow) : BaseService<IUserService, UserService>(uow), IUserService
{
    // TODO: Delete user profile picture on delete User

    public User GetUser(Guid guid)
    {
        AllowOnlyAdmins();

        return uow.Repository<User>().GetOrThrow(guid);
    }
}
