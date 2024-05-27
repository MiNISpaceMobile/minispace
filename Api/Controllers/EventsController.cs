using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Users;
using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Api.Controllers;

[Route("events")]
[ApiController]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("List all events")]
    public ActionResult<Paged<ListEventDto>> GetEvents([FromQuery] Paging paging,
        [FromQuery] IEnumerable<ParticipantsType>? participants = null,
        [FromQuery] IEnumerable<TimeType>? time = null,
        [FromQuery] string evNameFilter = "",
        string orgNameFilter = "", 
        PriceFilter priceFilter = PriceFilter.Any,
        StartTimeFilter startTimeFilter = StartTimeFilter.Any, bool onlyAvailablePlace = false)
    {
        var events = eventService.GetAll();
        events = Filter(events, evNameFilter, orgNameFilter, priceFilter, time, onlyAvailablePlace, participants);

        var paged = Paged<ListEventDto>.PageFrom(events.Select(e => e.ToListEventDto()),
            EventStateComparer.Instance, paging);

        return Ok(paged);
    }

    [HttpGet]
    [Route("details")]
    [SwaggerOperation("Details of given event")]
    public ActionResult GetEvent(Guid guid)
    {
        var @event = eventService.GetEvent(guid);
        return Ok(@event.ToDto());
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

    public enum ParticipantsType
    {
        To50,
        From50To100,
        Above100
    }

    public enum TimeType
    {
        Past,
        Current,
        Future
    }

    public enum PriceFilter
    {
        Any,
        Free,
        Paid
    }
    public enum StartTimeFilter
    {
        Any,
        Ended,
        Current,
        Incoming
    }

    private List<Event> Filter(List<Event> events, string evNameFilter, string orgNameFilter,
        PriceFilter priceFilter, IEnumerable<TimeType>? time, bool onlyAvailablePlace, IEnumerable<ParticipantsType>? participants)
    {
        List<Event> filtered = events;

        if (evNameFilter != string.Empty)
            filtered = filtered.FindAll(e => e.Title.Contains(evNameFilter));

        if (orgNameFilter != string.Empty)
        {
            var name = orgNameFilter.Split();
            string firstName = name[0];
            string lastName = string.Empty;
            if (name.Length > 1)
                lastName = name[1];
            filtered = filtered.FindAll(e => e.Organizer is not null && e.Organizer.FirstName.Contains(firstName) && e.Organizer.LastName.Contains(lastName));
        }

        if (priceFilter == PriceFilter.Free)
            filtered = filtered.FindAll(e => e.Fee is null);
        else if (priceFilter == PriceFilter.Paid)
            filtered = filtered.FindAll(e => e.Fee is not null);

        // Number of participants filter
        if(participants is not null && participants.Any() && participants.Count() < 3)
        {
            if(participants.Count() == 1)
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
        

        if (onlyAvailablePlace)
            filtered = filtered.FindAll(e => e.Capacity is null || (e.Capacity - e.Participants.Count > 0));

        return filtered;
    }
}
