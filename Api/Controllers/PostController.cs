using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Comments;
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

    public PostController(IPostService postService)
    {
        this.postService = postService;
    }

    [HttpGet]
    [Authorize]
    [Route("{id}")]
    [SwaggerOperation("Get post details")]
    public ActionResult<PostDto> GetPostDetails([FromRoute] Guid id)
    {
        var post = postService.AsUser(User.GetGuid()).GetPost(id);
        return Ok(post.ToDto());
    }

    [HttpGet]
    [Authorize]
    [Route("user")]
    [SwaggerOperation("List user's subscribed events' posts")]
    public ActionResult<Paged<ListPostDto>> GetUserEventsPosts([FromQuery] Paging paging, [FromQuery] bool showFriendsPosts = false)
    {
        var user = postService.AsUser(User.GetGuid()).ActingUser;
        HashSet<Event> events = new HashSet<Event>(new EventEqualityComparer());
        events.UnionWith(user!.SubscribedEvents);
        events.UnionWith(user.JoinedEvents);
        events.UnionWith(user.OrganizedEvents);

        if (showFriendsPosts)
        {
            var friends = user.Friends;
            foreach (var f in friends)
            {
                events.UnionWith(f.SubscribedEvents);
                events.UnionWith(f.JoinedEvents);
            }
        }

        IEnumerable<Post> posts = Enumerable.Empty<Post>();
        foreach (var e in events)
            posts = posts.Concat(e.Posts);

        return Paged<ListPostDto>.PageFrom(posts.OrderByDescending(p => p.CreationDate).Select(p => p.ToListPostDto()), DummyComparer.Instance, paging);
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

    [HttpGet]
    [Authorize]
    [Route("{id}/comments")]
    [SwaggerOperation("List comments for given post")]
    public ActionResult<Paged<CommentDto>> GetPostComments([FromQuery] Paging paging, [FromRoute] Guid id)
    {
        var comments = postService.AsUser(User.GetGuid()).GetPost(id).Comments;
        return Paged<CommentDto>.PageFrom(comments.AsEnumerable().Where(c => c.InResponeseToId is null).Select(c => c.ToDto()), DTO.Comments.CreationDateComparer.Instance, paging);
    }
}
