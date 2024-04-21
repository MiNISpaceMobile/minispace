using Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// TODO: Implement controller
[Route("events")]
[ApiController]
public class EventsController : ControllerBase
{
    /// <summary>
    /// Create a new event with given parameters.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateEvent(CreateEvent createEvent)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets event corresponding to provided uuid.
    /// </summary>
    [HttpGet("{uuid}")]
    [ProducesResponseType<EventDto>(StatusCodes.Status200OK)]
    public ActionResult<EventDto> GetEvent(Guid uuid)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Searches for events with provided filters and sorting, returns a single pageable object of events.
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType<PagedResponse<EventDto>>(StatusCodes.Status200OK)]
    public PagedResponse<EventDto> SearchEvents(EventSearchDetails details)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Registers a user determined by userId included in jwt for the event referenced by id path parameter.
    /// </summary>
    [HttpPost("{id}/participants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult RegisterForEvent(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a user determined by userId included in jwt from the participants list of the event referenced by id path parameter.
    /// </summary>
    [HttpDelete("{id}/participants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult RemoveFromEvent(int id)
    {
        throw new NotImplementedException();
    }
}