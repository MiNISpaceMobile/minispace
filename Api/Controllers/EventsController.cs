﻿using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Users;
using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    [Produces("application/json")]
    public ActionResult GetEvents(int pageNr, int pageSize, string evNameFilter = "", string orgNameFilter = "", 
        PriceFilter priceFilter = PriceFilter.Any, int minCapacityFilter = 0, int maxCapacityFilter = int.MaxValue, StartTimeFilter startTimeFilter = StartTimeFilter.Current, bool onlyAvailablePlace = false)
    {
        var events = eventService.GetAll();
        events = Filter(events, evNameFilter, orgNameFilter, priceFilter, minCapacityFilter, maxCapacityFilter, startTimeFilter, onlyAvailablePlace);

        bool isLastPage = false;
        int pagesCount = (int)Math.Ceiling((double)events.Count / pageSize) - 1;
        if (pageNr >= pagesCount)
            isLastPage = true;

        var page = events.Skip(pageNr * pageSize).Take(pageSize);

        return Ok(new { page = page.Select(e => e.ToListEventDto()), isLastPage = isLastPage });
    }

    [HttpGet]
    [Route("event")]
    [Produces("application/json")]
    public ActionResult GetEvent(Guid guid)
    {
        Event @event;
        try
        {
            @event = eventService.GetEvent(guid);
        }
        catch (InvalidGuidException ex) 
        {
            return BadRequest(ex.Message);
        }

        return Ok(@event.ToDto());
    }

    public enum PriceFilter
    {
        Any,
        Free,
        Paid
    }
    public enum StartTimeFilter
    {
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
        }

        filtered = filtered.FindAll(e => e.Capacity is null || (e.Capacity - e.Participants.Count > 0));

        return filtered;
    }
}