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

    public Event GetEvent(Guid guid)
    {
        Event? @event = uow.Repository<Event>().Get(guid);
        if (@event is null)
            throw new ArgumentException("Nonexistent event");

        return @event;
    }

    public Event CreateEvent(Guid studentGuid, string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee)
    {
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null)
            throw new ArgumentException("Nonexistent student");

        Event @event = new Event(student, title, description, category, publicationDate,
            startDate, endDate, location, capacity, fee);
        uow.Repository<Event>().Add(@event);
        uow.Commit();
        return @event;
    }

    /// <summary>
    /// Assignes values of given event to event with the same guid existing in db 
    /// </summary>
    /// <param name="newEvent"></param>
    /// <exception cref="ArgumentException"></exception>
    public void UpdateEvent(Event newEvent)
    {
        Event? currEvent = uow.Repository<Event>().Get(newEvent.Guid);
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

    /// <summary>
    /// Tries to add student to participants of event. Removes from Interested list.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <param name="studentGuid"></param>
    /// <returns>
    /// true if operation was successfull, false if participants number reached maximum or student is already participating
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public bool TryAddParticipant(Guid eventGuid, Guid studentGuid)
    {
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null || @event is null)
            throw new ArgumentException("Nonexistent object");

        // Full event
        if (@event.Capacity is not null && @event.Participants.Count == @event.Capacity)
            return false;
        // Already participating
        if (@event.Participants.Contains(student))
            return false;

        @event.Interested.Remove(student);
        if (!student.SubscribedEvents.Contains(@event))
            student.SubscribedEvents.Add(@event);
        @event.Participants.Add(student);
        return true;
    }

    /// <summary>
    /// Tries to add student to interested of event. Removes from Participants list.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <param name="studentGuid"></param>
    /// <returns>
    /// true if operation was successfull, false if student is already interested
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public bool TryAddInterested(Guid eventGuid, Guid studentGuid)
    {
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null || @event is null)
            throw new ArgumentException("Nonexistent object");
        
        // Already interested
        if (@event.Interested.Contains(student))
            return false;

        @event.Participants.Remove(student);
        if (!student.SubscribedEvents.Contains(@event))
            student.SubscribedEvents.Add(@event);
        @event.Interested.Add(student);
        return true;
    }
}
