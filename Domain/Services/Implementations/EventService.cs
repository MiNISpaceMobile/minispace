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

    public void UpdateEvent(Event newEvent)
    {
        var currEvent = uow.Repository<Event>().Get(newEvent.Guid);
        if (currEvent is null)
            throw new ArgumentException();

        currEvent.Title = newEvent.Title;
        currEvent.Description = newEvent.Description;
        currEvent.Location = newEvent.Location;
        currEvent.StartDate = newEvent.StartDate;
        currEvent.EndDate = newEvent.EndDate;
        currEvent.Category = newEvent.Category;
        currEvent.Capacity = newEvent.Capacity;
        currEvent.Fee = newEvent.Fee;

        uow.Commit();
    }
}
