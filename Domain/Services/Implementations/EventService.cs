using Domain.Abstractions;
using Domain.DataModel;
using System;

namespace Domain.Services;

public class EventService : IEventService
{
    private IUnitOfWork uow;

    public EventService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public Event? GetEvent(Guid guid) => uow.Repository<Event>().Get(guid);

    public Event CreateEvent(Guid studentGuid, string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee)
    {
        Student student = uow.Repository<Student>().Get(studentGuid);
        if (student is null)
            throw new ArgumentException("Nonexistent student");

        Event @event = new Event(student, title, description, category, publicationDate,
            startDate, endDate, location, capacity, fee);
        uow.Repository<Event>().Add(@event);
        uow.Commit();
        return @event;
    }

    public void UpdateEvent(Event newEvent)
    {
        var currEvent = uow.Repository<Event>().Get(newEvent.Guid);
        if (currEvent is null)
            throw new ArgumentException("Nonexistent event");

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
