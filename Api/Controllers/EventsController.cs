using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Posts;
using Domain.DataModel;
using Domain.Services;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("events")]
[ApiController]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("List all events")]
    public ActionResult<Paged<ListEventDto>> GetEvents([FromQuery] Paging paging, [FromQuery] GetEventsFilters f)
    {
        var events = eventService.AsUser(User.TryGetGuid()).GetAll();
        events = Filter(events,
            f.Time?.OfType<TimeType>(), f.Participants?.OfType<ParticipantsType>(),
            f.Price?.OfType<PriceType>(), f.EventName, f.OrganizerName, f.OnlyAvailablePlace,
            f.OrganizedByMe, eventService.ActingUser);

        var paged = Paged<ListEventDto>.PageFrom(events.Select(e => e.ToListEventDto()),
            EventStateComparer.Instance, paging);

        return Ok(paged);
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation("Create event")]
    public ActionResult<EventDto> CreateEvent(CreateEvent newEvent)
    {
        var @event = eventService.AsUser(User.GetGuid()).CreateEvent(newEvent.Title, newEvent.Description, newEvent.EventCategory,
            newEvent.PublicationDate, newEvent.StartDate, newEvent.EndDate, newEvent.Location, newEvent.Capacity, newEvent.Fee);
        return Ok(@event.ToDto(eventService.ActingUser));
    }

    [HttpGet]
    [Route("{id}")]
    [SwaggerOperation("Details of given event")]
    public ActionResult<EventDto> GetEvent(Guid id)
    {
        var @event = eventService.AsUser(User.TryGetGuid()).GetEvent(id);
        return Ok(@event.ToDto(eventService.ActingUser));
    }

    [HttpGet]
    [Route("{id}/posts")]
    [SwaggerOperation("List event's posts")]
    public ActionResult<Paged<PostDto>> GetEventPosts([FromQuery] Paging paging, [FromRoute] Guid id)
    {
        var @event = eventService.AsUser(User.GetGuid()).GetEvent(id);
        return Paged<PostDto>.PageFrom(@event.Posts.Select(p => p.ToDto(User.GetGuid())), CreationDateComparer.Instance, paging);
    }

    [HttpDelete]
    [Authorize]
    [Route("{id}")]
    [SwaggerOperation("Delete event")]
    public ActionResult DeleteEvent(Guid id)
    {
        eventService.AsUser(User.GetGuid()).DeleteEvent(id);
        return Ok();
    }

    [HttpPost]
    [Authorize]
    [Route("{id}/participants")]
    [SwaggerOperation("Register for event")]
    public ActionResult RegisterForEvent(Guid id)
    {
        return eventService.AsUser(User.GetGuid()).TryAddParticipant(id) ?
            Ok() : BadRequest("You can't register for this event");
    }

    [HttpDelete]
    [Authorize]
    [Route("{id}/participants")]
    [SwaggerOperation("Unregister from event")]
    public ActionResult UnregisterFromEvent(Guid id)
    {
        return eventService.AsUser(User.GetGuid()).TryRemoveParticipant(id) ?
            Ok() : BadRequest("You aren't registered for this event");
    }

    [HttpPost]
    [Authorize]
    [Route("{id}/interested")]
    [SwaggerOperation("Show interest in event")]
    public ActionResult ShowInterestInEvent(Guid id)
    {
        return eventService.AsUser(User.GetGuid()).TryAddInterested(id) ?
            Ok() : BadRequest("You are already interested in this event");
    }

    [HttpDelete]
    [Authorize]
    [Route("{id}/interested")]
    [SwaggerOperation("Remove interest from event")]
    public ActionResult RemoveInterestInEvent(Guid id)
    {
        return eventService.AsUser(User.GetGuid()).TryRemoveInterested(id) ?
            Ok() : BadRequest("You aren't interested in this event");
    }

    [HttpPost]
    [Authorize]
    [Route("{id}/rate")]
    [SwaggerOperation("Rate ended event")]
    public ActionResult<FeedbackDto> AddFeedback(Guid id, [FromBody] AddRating request)
    {
        return Ok(eventService.AsUser(User.GetGuid()).AddFeedback(id, request.Rating).ToDto());
    }

    private static IEnumerable<Event> Filter(IEnumerable<Event> events, IEnumerable<TimeType>? time,
         IEnumerable<ParticipantsType>? participants, IEnumerable<PriceType>? price,
         string? evNameFilter, string? orgNameFilter, bool onlyAvailablePlace,
         bool organizedByMe, User? user)
    {
        // Event name filter
        if (!string.IsNullOrEmpty(evNameFilter))
            events = events.Where(e => e.Title.Contains(evNameFilter, StringComparison.InvariantCultureIgnoreCase));

        // Organizer name filter
        if (!string.IsNullOrEmpty(orgNameFilter))
        {
            var name = orgNameFilter.Split();
            string firstName = name[0];
            string lastName = string.Empty;
            if (name.Length > 1)
                lastName = name[1];
            events = events.Where(e => e.Organizer is not null &&
            e.Organizer.FirstName.Contains(firstName, StringComparison.InvariantCultureIgnoreCase) &&
            e.Organizer.LastName.Contains(lastName, StringComparison.InvariantCultureIgnoreCase));
        }

        // Number of participants filter
        if (participants is not null && participants.Any())
        {
            ParticipantsType countToType(int i) => i switch
            {
                <= 50 => ParticipantsType.To50,
                >= 50 and <= 100 => ParticipantsType.From50To100,
                _ => ParticipantsType.Above100
            };
            events = events.Where(x => participants.Contains(countToType(x.Participants.Count)));
        }

        // Time filter
        if (time is not null && time.Any())
        {
            TimeType startEndToTimeType(DateTime start, DateTime end) => (start, end) switch
            {
                _ when end <= DateTime.Now => TimeType.Past,
                _ when start <= DateTime.Now && end >= DateTime.Now => TimeType.Current,
                _ => TimeType.Future
            };
            events = events.Where(x => time.Contains(startEndToTimeType(x.StartDate, x.EndDate)));
        }

        // Price filter
        if (price is not null && price.Any() && price.Count() < 2)
            events = events.Where(x => price.First() == PriceType.Free ? x.Fee is null || x.Fee == 0 : x.Fee is not null && x.Fee > 0);

        // Only events with available placces
        if (onlyAvailablePlace)
            events = events.Where(e => e.Capacity is null || (e.Capacity - e.Participants.Count > 0));

        // Only events organized by acting user
        if (organizedByMe)
            events = events.Where(e => user is not null && user.Guid == e.Organizer?.Guid);

        return events;
    }
}
