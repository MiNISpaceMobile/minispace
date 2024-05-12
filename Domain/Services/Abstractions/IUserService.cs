using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IUserService : IBaseService<IUserService>
{
    public User GetUser(Guid guid);
}
