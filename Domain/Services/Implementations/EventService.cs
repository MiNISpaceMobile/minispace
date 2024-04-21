using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using System;

namespace Domain.Services;

public class EventService : IEventService
{
    private IUnitOfWork uow;
    private PostService postService;

    public EventService(IUnitOfWork uow)
    {
        this.uow = uow;
        postService = new PostService(uow);
    }

    public Event GetEvent(Guid guid)
    {
        Event? @event = uow.Repository<Event>().Get(guid);
        if (@event is null)
            throw new InvalidGuidException<Event>();

        return @event;
    }

    public Event CreateEvent(Guid studentGuid, string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee)
    {
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null)
            throw new InvalidGuidException<Event>();

        Event @event = new Event(student, title, description, category, publicationDate,
            startDate, endDate, location, capacity, fee);
        uow.Repository<Event>().Add(@event);
        uow.Commit();
        return @event;
    }

    /// <summary>
    /// Deletes event and it's posts
    /// </summary>
    /// <param name="guid"></param>
    public void DeleteEvent(Guid guid)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(guid);

        while (@event.Posts.Count > 0) 
            postService.DeletePost(@event.Posts.First().Guid);
        while (@event.Participants.Count > 0)
            TryRemoveParticipant(@event.Guid, @event.Participants.First().Guid);
        while (@event.Interested.Count > 0)
            TryRemoveInterested(@event.Guid, @event.Interested.First().Guid);

        uow.Repository<Event>().TryDelete(guid);

        uow.Commit();
    }

    /// <summary>
    /// Assignes values of given event to event with the same guid existing in db 
    /// </summary>
    /// <param name="newEvent"></param>
    /// <exception cref="InvalidGuidException"></exception>
    public void UpdateEvent(Event newEvent)
    {
        Event? currEvent = uow.Repository<Event>().Get(newEvent.Guid);
        if (currEvent is null)
            throw new InvalidGuidException<Event>();

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
    /// <exception cref="InvalidGuidException"></exception>
    public bool TryAddParticipant(Guid eventGuid, Guid studentGuid)
    {
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null || @event is null)
            throw new InvalidGuidException();

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

        uow.Commit();

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
    /// <exception cref="InvalidGuidException"></exception>
    public bool TryAddInterested(Guid eventGuid, Guid studentGuid)
    {
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null || @event is null)
            throw new InvalidGuidException();
        
        // Already interested
        if (@event.Interested.Contains(student))
            return false;

        @event.Participants.Remove(student);
        if (!student.SubscribedEvents.Contains(@event))
            student.SubscribedEvents.Add(@event);
        @event.Interested.Add(student);

        uow.Commit();

        return true;
    }

    /// <summary>
    /// Tries to remove student from participants of event.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <param name="studentGuid"></param>
    /// <returns>
    /// true if operation was successfull, false if student is already not participating
    /// </returns>
    /// <exception cref="InvalidGuidException"></exception>
    public bool TryRemoveParticipant(Guid eventGuid, Guid studentGuid)
    {
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null || @event is null)
            throw new InvalidGuidException();

        // Already not participating
        if (!@event.Participants.Contains(student))
            return false;

        @event.Participants.Remove(student);
        student.SubscribedEvents.Remove(@event);

        uow.Commit();

        return true;
    }

    /// <summary>
    /// Tries to remove student from interested of event.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <param name="studentGuid"></param>
    /// <returns>
    /// true if operation was successfull, false if student is already not interested
    /// </returns>
    /// <exception cref="InvalidGuidException"></exception>
    public bool TryRemoveInterested(Guid eventGuid, Guid studentGuid)
    {
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        Student? student = uow.Repository<Student>().Get(studentGuid);
        if (student is null || @event is null)
            throw new InvalidGuidException();

        // Already not interested
        if (!@event.Interested.Contains(student))
            return false;

        @event.Interested.Remove(student);
        student.SubscribedEvents.Remove(@event);

        uow.Commit();

        return true;
    }

    public Feedback AddFeedback(Guid eventGuid, Guid authorGuid, string content)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);
        Student author = uow.Repository<Student>().GetOrThrow(authorGuid);
        if (content == string.Empty)
            throw new EmptyContentException();

        if (@event.Feedback.FirstOrDefault(f => f.Author.Guid == authorGuid) is not null)
            throw new InvalidOperationException("This Student have already given Feedback to this Event");

        Feedback feedback = new Feedback(author, @event, content);
        //uow.Repository<Feedback>().Add(feedback);
        @event.Feedback.Add(feedback);

        uow.Commit();
        return feedback;
    }
}
