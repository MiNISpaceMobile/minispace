using Domain.Abstractions;
using Domain.DataModel;

namespace Domain.Services;

public class EventService
{
    private IUnitOfWork uow;

    public EventService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public Event? GetEvent(Guid guid) => uow.Repository<Event>().Get(guid);
}
