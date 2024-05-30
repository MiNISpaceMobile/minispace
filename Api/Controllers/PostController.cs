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

[Route("posts")]
[ApiController]
public class PostController : ControllerBase
{
    private IPostService postService;
    private IUserService userService;

    public PostController(IPostService postService, IUserService userService)
    {
        this.postService = postService;
        this.userService = userService;
    }

    [HttpGet]
    [Authorize]
    [Route("user")]
    [SwaggerOperation("List user's subscribed events' posts")]
    public ActionResult<Paged<PostDto>> GetUserEventsPosts([FromQuery] Paging paging, [FromQuery] bool showFrindsPosts = false)
    {
        var user = userService.AsUser(User.GetGuid()).GetUser();
        HashSet<Event> events = new HashSet<Event>(new EventEqualityComparer());
        events.UnionWith(user.SubscribedEvents);

        if (showFrindsPosts)
        {
            var friends = user.Friends;
            foreach (var f in friends)
                events.UnionWith(f.SubscribedEvents);
        }

        IEnumerable<Post> posts = Enumerable.Empty<Post>();
        foreach (var e in events)
            posts = posts.Concat(e.Posts);

        return Paged<PostDto>.PageFrom(posts.Select(p => p.ToDto()), CreationDateComparer.Instance, paging);
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation("Create post")]
    public ActionResult<PostDto> CreatePost(CreatePost post)
    {
        Post newPost = postService.AsUser(User.GetGuid()).CreatePost(post.EventGuid, post.Title, post.Content);
        return Ok(newPost.ToDto());
    }

    [HttpDelete]
    [Authorize]
    [Route("{id}")]
    [SwaggerOperation("Delete post")]
    public ActionResult DeleteEvent(Guid id)
    {
        postService.AsUser(User.GetGuid()).DeletePost(id);
        return Ok();
    }
}
