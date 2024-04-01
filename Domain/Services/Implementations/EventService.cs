using Domain.Abstractions;
using Domain.DataModel;

namespace Domain.Services;

public class EventService : IEventService
{
    private IUnitOfWork uow;

    public EventService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public Event? GetEvent(Guid guid) => uow.Repository<Event>().Get(guid);

    public void CreateEvent(Event newEvent)
    {
        uow.Repository<Event>().Add(newEvent);
        uow.Commit();
    }
    
}
