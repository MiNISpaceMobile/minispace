using Api.DTO;
using Api.DTO.Events;
using Domain.DataModel;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.Tracing;

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
        events = Filter(events, f.EventName, f.OrganizerName, f.Time, f.OnlyAvailablePlace, f.Participants, f.Price);

        var paged = Paged<ListEventDto>.PageFrom(events.Select(e => e.ToListEventDto()),
            EventStateComparer.Instance, paging);

        return Ok(paged);
    }

    [HttpGet]
    [Route("details")]
    [SwaggerOperation("Details of given event")]
    public ActionResult<EventDto> GetEvent(Guid guid)
    {
        var @event = eventService.AsUser(User.TryGetGuid()).GetEvent(guid);
        return Ok(@event.ToDto(eventService.ActingUser!));
    }

    [HttpPost]
    [Authorize]
    [Route("create")]
    [SwaggerOperation("Create event")]
    public ActionResult CreateEvent(CreateEvent newEvent)
    {
        eventService.AsUser(User.GetGuid()).CreateEvent(newEvent.Title, newEvent.Description, newEvent.EventCategory, newEvent.PublicationDate, newEvent.StartDate, newEvent.EndDate, newEvent.Location, newEvent.Capacity, newEvent.Fee);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    [Route("delete")]
    [SwaggerOperation("Delete event")]
    public ActionResult DeleteEvent(Guid eventGuid)
    {
        eventService.AsUser(User.GetGuid()).DeleteEvent(eventGuid);
        return Ok();
    }

    [HttpPost]
    [Authorize]
    [Route("{eventGuid}/participate")]
    [SwaggerOperation("Register for event")]
    public ActionResult<bool> RegisterForEvent(Guid eventGuid)
    {
        return Ok(eventService.AsUser(User.GetGuid()).TryAddParticipant(eventGuid));
    }

    [HttpDelete]
    [Authorize]
    [Route("{eventGuid}/participate")]
    [SwaggerOperation("Unregister from event")]
    public ActionResult<bool> UnregisterFromEvent(Guid eventGuid)
    {
        return Ok(eventService.AsUser(User.GetGuid()).TryRemoveParticipant(eventGuid));
    }

    [HttpPost]
    [Authorize]
    [Route("{eventGuid}/interest")]
    [SwaggerOperation("Show interest in event")]
    public ActionResult<bool> ShowInterestInEvent(Guid eventGuid)
    {
        return Ok(eventService.AsUser(User.GetGuid()).TryAddInterested(eventGuid));
    }

    [HttpDelete]
    [Authorize]
    [Route("{eventGuid}/interest")]
    [SwaggerOperation("Remove interest from event")]
    public ActionResult<bool> RemoveInterestInEvent(Guid eventGuid)
    {
        return Ok(eventService.AsUser(User.GetGuid()).TryRemoveInterested(eventGuid));
    }

    [HttpPost]
    [Authorize]
    [Route("{eventGuid}/feedback")]
    public ActionResult<Feedback> AddFeedback(Guid eventGuid, int rating)
    {
        return Ok(eventService.AsUser(User.GetGuid()).AddFeedback(eventGuid, rating));
    }

    private static List<Event> Filter(List<Event> events, string evNameFilter, string orgNameFilter, IEnumerable<TimeType>? time,
        bool onlyAvailablePlace, IEnumerable<ParticipantsType>? participants, IEnumerable<PriceType>? price)
    {
        List<Event> filtered = events;

        // Event name filter
        if (evNameFilter != string.Empty)
            filtered = filtered.FindAll(e => e.Title.Contains(evNameFilter));

        // Organizer name filter
        if (orgNameFilter != string.Empty)
        {
            var name = orgNameFilter.Split();
            string firstName = name[0];
            string lastName = string.Empty;
            if (name.Length > 1)
                lastName = name[1];
            filtered = filtered.FindAll(e => e.Organizer is not null && e.Organizer.FirstName.Contains(firstName) && e.Organizer.LastName.Contains(lastName));
        }

        // Number of participants filter
        if (participants is not null && participants.Any() && participants.Count() < 3)
        {
            if (participants.Count() == 1)
            {
                filtered = participants.First() switch
                {
                    ParticipantsType.To50 => filtered.FindAll(x => x.Participants.Count <= 50 && x.Participants.Count >= 0),
                    ParticipantsType.From50To100 => filtered.FindAll(x => x.Participants.Count >= 50 && x.Participants.Count <= 100),
                    ParticipantsType.Above100 => filtered.FindAll(x => x.Participants.Count >= 100),
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                if (!participants.Contains(ParticipantsType.To50))
                    filtered = filtered.FindAll(x => x.Participants.Count >= 50);
                else if (!participants.Contains(ParticipantsType.Above100))
                    filtered = filtered.FindAll(x => x.Participants.Count >= 0 && x.Participants.Count <= 100);
                else if (!participants.Contains(ParticipantsType.From50To100))
                    filtered = filtered.FindAll(x => (x.Participants.Count >= 0 && x.Participants.Count <= 50) || (x.Participants.Count >= 100));
            }
        }

        // Time filter
        if (time is not null && time.Any() && time.Count() < 3)
        {
            if (time.Count() == 1)
            {
                filtered = time.First() switch
                {
                    TimeType.Past => filtered.FindAll(x => x.EndDate <= DateTime.Now),
                    TimeType.Current => filtered.FindAll(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now),
                    TimeType.Future => filtered.FindAll(x => x.StartDate >= DateTime.Now),
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                if (!time.Contains(TimeType.Past))
                    filtered = filtered.FindAll(x => x.EndDate >= DateTime.Now);
                else if (!time.Contains(TimeType.Future))
                    filtered = filtered.FindAll(x => x.StartDate <= DateTime.Now);
                else if (!time.Contains(TimeType.Current))
                    filtered = filtered.FindAll(x => (x.EndDate <= DateTime.Now) || (x.StartDate >= DateTime.Now));
            }
        }

        // Price filter
        if (price is not null && price.Any() && price.Count() < 2)
            filtered = filtered.FindAll(x => price.First() == PriceType.Free ? x.Fee is null || x.Fee == 0 : x.Fee is not null || x.Fee > 0);


        if (onlyAvailablePlace)
            filtered = filtered.FindAll(e => e.Capacity is null || (e.Capacity - e.Participants.Count > 0));

        return filtered;
    }
}
