using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Posts;
using Api.DTO.Users;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("posts")]
[ApiController]
public class PostController : ControllerBase
{
    private IPostService postService;
    private IEventService eventService;

    public PostController(IPostService postService, IEventService eventService)
    {
        this.postService = postService;
        this.eventService = eventService;
    }

    [HttpGet]
    [Authorize]
    [Route("user")]
    [SwaggerOperation("List user's subscribed events' posts")]
    public ActionResult<Paged<PostDto>> GetUserEventsPosts([FromQuery] Paging paging, Guid userGuid, bool showAlsoInterested)
    {
        var posts = postService.AsUser(User.GetGuid()).GetUsersPosts(userGuid);
        if (!showAlsoInterested)
            posts = posts.FindAll(p => p.Event.Participants.FirstOrDefault(part => part.Guid == userGuid) is not null);
        return Paged<PostDto>.PageFrom(posts.Select(p => p.ToDto()), CreationDateComparer.Instance, paging);
    }

    [HttpGet]
    [Authorize]
    [Route("event")]
    [SwaggerOperation("List event's posts")]
    public ActionResult<Paged<PostDto>> GetEventPosts([FromQuery] Paging paging, Guid eventGuid)
    {
        var @event = eventService.AsUser(User.GetGuid()).GetEvent(eventGuid);
        return Paged<PostDto>.PageFrom(@event.Posts.Select(p => p.ToDto()), CreationDateComparer.Instance, paging);
    }

    [HttpPost]
    [Authorize]
    [Route("create")]
    [SwaggerOperation("Create post")]
    public ActionResult CreatePost(CreatePost post)
    {
        postService.AsUser(User.GetGuid()).CreatePost(post.EventGuid, post.Content);
        return Ok();
    }
}
