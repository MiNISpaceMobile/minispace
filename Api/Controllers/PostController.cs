using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Comments;
using Api.DTO.Posts;
using Domain.DataModel;
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
        return Ok(post.ToDto(User.GetGuid()));
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
        return Ok(newPost.ToDto(User.GetGuid()));
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
        var comments = postService.AsUser(User.GetGuid()).GetPost(id).Comments
            .Where(c => c.InResponeseToId is null);
        return Paged<CommentDto>.PageFrom(comments.Select(c => c.ToDto(User.GetGuid())),
            DTO.Comments.CreationDateComparer.Instance, paging);
    }

    [HttpGet]
    [Authorize]
    [Route("{id}/reactions")]
    [SwaggerOperation("List all post's reactions")]
    public ActionResult<Paged<ReactionDto>> GetPostReactions([FromRoute] Guid id, [FromQuery] Paging paging)
    {
        var reactions = postService.AsUser(User.GetGuid()).GetPost(id).Reactions;
        var paged = Paged<ReactionDto>.PageFrom(reactions.Select(x => x.ToDto(postService.ActingUser!)),
            DTO.Posts.ByFriendComparer.Instance, paging);
        return Ok(paged);
    }

    [HttpPatch]
    [Authorize]
    [Route("{id}/reactions")]
    [SwaggerOperation("Set acting user's reaction to post")]
    public ActionResult PatchReaction([FromRoute] Guid id, [FromBody] SetReaction reaction)
    {
        postService.AsUser(User.GetGuid()).SetReaction(id, reaction.Type);
        return Ok();
    }
}
