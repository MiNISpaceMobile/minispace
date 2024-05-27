using Domain.Abstractions;
using Domain.DataModel;

namespace Infrastructure.PingResponders;

public class PongPingResponder(IUnitOfWork uow) : IPingResponder
{
    public string Response()
    {
        return "Pong";
    }
    public string Response(Guid guid)
    {
        var user = uow.Repository<User>().Get(guid);

        if (user is null)
            return "Pong to noexistent";
        
        return $"Pong to {user.FirstName} {user.LastName}";
    }
}
