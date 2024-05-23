using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class EventService(IUnitOfWork uow) : BaseService<IEventService, EventService>(uow), IEventService
{
    private PostService postService = new PostService(uow);

    public Event GetEvent(Guid guid)
    {
        AllowEveryone();

        return uow.Repository<Event>().GetOrThrow(guid);
    }

    public Event CreateEvent(string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee)
    {
        AllowOnlyOrganizers();

        User author = ActingUser!;

        Event @event = new Event(author, title, description, category, publicationDate,
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

        AllowOnlyUser(@event.Organizer);

        while (@event.Posts.Count > 0) 
            postService.AsUser(@event.Organizer?.Guid).DeletePost(@event.Posts.First().Guid);
        while (@event.Participants.Count > 0)
            TryRemoveParticipant(@event.Guid);
        while (@event.Interested.Count > 0)
            TryRemoveInterested(@event.Guid);

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
        Event currEvent = uow.Repository<Event>().GetOrThrow(newEvent.Guid);

        AllowOnlyUser(currEvent.Organizer);

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
    public bool TryAddParticipant(Guid eventGuid)
    {
        AllowOnlyLoggedIn();

        User student = ActingUser!;

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

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
    /// <returns>
    /// true if operation was successfull, false if student is already interested
    /// </returns>
    /// <exception cref="InvalidGuidException"></exception>
    public bool TryAddInterested(Guid eventGuid)
    {
        AllowOnlyLoggedIn();

        User student = ActingUser!;

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);
        
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
    /// <returns>
    /// true if operation was successfull, false if student is already not participating
    /// </returns>
    /// <exception cref="InvalidGuidException"></exception>
    public bool TryRemoveParticipant(Guid eventGuid)
    {
        AllowOnlyLoggedIn();

        User student = ActingUser!;

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

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
    public bool TryRemoveInterested(Guid eventGuid)
    {
        AllowOnlyLoggedIn();

        User student = ActingUser!;

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        // Already not interested
        if (!@event.Interested.Contains(student))
            return false;

        @event.Interested.Remove(student);
        student.SubscribedEvents.Remove(@event);

        uow.Commit();

        return true;
    }

    public Feedback AddFeedback(Guid eventGuid, string content)
    {
        AllowOnlyLoggedIn();

        User author = ActingUser!;

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        if (string.IsNullOrEmpty(content))
            throw new EmptyContentException();

        if (@event.Feedback.FirstOrDefault(f => f.Author.Guid == author.Guid) is not null)
            throw new InvalidOperationException("This Student have already given Feedback to this Event");

        Feedback feedback = new Feedback(author, @event, content);
        //uow.Repository<Feedback>().Add(feedback);
        @event.Feedback.Add(feedback);

        uow.Commit();
        return feedback;
    }
}
