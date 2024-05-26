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

namespace Api.Controllers;

[Route("events")]
[ApiController]
public class EventsController : ControllerBase
{
    private IEventService eventService;

    public EventsController(IEventService eventService)
    {
        this.eventService = eventService;
    }


    [HttpGet]
    [SwaggerOperation("List all events")]
    public ActionResult<Paged<ListEventDto>> GetEvents([FromQuery] Paging paging, string evNameFilter = "", string orgNameFilter = "", 
        PriceFilter priceFilter = PriceFilter.Any, int minCapacityFilter = 0, int maxCapacityFilter = int.MaxValue, StartTimeFilter startTimeFilter = StartTimeFilter.Any, bool onlyAvailablePlace = false)
    {
        var events = eventService.GetAll();
        events = Filter(events, evNameFilter, orgNameFilter, priceFilter, minCapacityFilter, maxCapacityFilter, startTimeFilter, onlyAvailablePlace);

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
    [SwaggerOperation("Create post")]
    public ActionResult CreateEvent(CreateEvent newEvent)
    {
        EventCategory cat;
        if (!Enum.TryParse(newEvent.EventCategory, out cat))
            return BadRequest("Nonexistent category");
        eventService.AsUser(User.GetGuid()).CreateEvent(newEvent.Title, newEvent.Description, cat, newEvent.PublicationDate, newEvent.StartDate, newEvent.EndDate, newEvent.Location, newEvent.Capacity, newEvent.Fee);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    [Route("delete")]
    [SwaggerOperation("Delete post")]
    public ActionResult DeleteEvent(Guid eventGuid)
    {
        eventService.AsUser(User.GetGuid()).DeleteEvent(eventGuid);
        return Ok();
    }

    [HttpPost]
    [Authorize]
    [Route("create")]
    [SwaggerOperation("Create post")]
    public ActionResult CreateEvent(CreateEvent newEvent)
    {
        EventCategory cat;
        if (!Enum.TryParse(newEvent.EventCategory, out cat))
            return BadRequest("Nonexistent category");
        eventService.AsUser(User.GetGuid()).CreateEvent(newEvent.Title, newEvent.Description, cat, newEvent.PublicationDate, newEvent.StartDate, newEvent.EndDate, newEvent.Location, newEvent.Capacity, newEvent.Fee);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    [Route("delete")]
    [SwaggerOperation("Delete post")]
    public ActionResult DeleteEvent(Guid eventGuid)
    {
        eventService.AsUser(User.GetGuid()).DeleteEvent(eventGuid);
        return Ok();
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
        PriceFilter priceFilter, int minCapacityFilter, int maxCapacityFilter, StartTimeFilter startTimeFilter, bool onlyAvailablePlace)
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

        filtered = filtered.FindAll(e => e.Capacity is null || (e.Capacity >= minCapacityFilter && e.Capacity <= maxCapacityFilter));

        switch (startTimeFilter)
        {
            case StartTimeFilter.Ended:
                filtered = filtered.FindAll(e => e.EndDate < DateTime.Now);
                break;
            case StartTimeFilter.Current:
                filtered = filtered.FindAll(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now);
                break;
            case StartTimeFilter.Incoming:
                filtered = filtered.FindAll(e => e.StartDate > DateTime.Now);
                break;
            default:
                break;
        }

        if (onlyAvailablePlace)
            filtered = filtered.FindAll(e => e.Capacity is null || (e.Capacity - e.Participants.Count > 0));

        return filtered;
    }
}
