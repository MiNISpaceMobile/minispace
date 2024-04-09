using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IUserService
{
    public User GetUser(Guid guid);
}
