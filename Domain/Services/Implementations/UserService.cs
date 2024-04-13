using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class UserService : IUserService
{
    private IUnitOfWork uow;

    public UserService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public User GetUser(Guid guid) => uow.Repository<User>().GetOrThrow(guid);
}
