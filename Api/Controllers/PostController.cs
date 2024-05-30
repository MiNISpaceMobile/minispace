using Api.DTO;
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
    [Route("user")]
    [SwaggerOperation("List user's subscribed events' posts")]
    public ActionResult<Paged<PostDto>> GetUserEventsPosts([FromQuery] Paging paging, [FromQuery] bool showAlsoInterested)
    {
        var posts = postService.AsUser(User.GetGuid()).GetUsersPosts();
        if (!showAlsoInterested)
            posts = posts.FindAll(p => p.Event.Participants.Any(part => part.Guid == User.GetGuid()));
        return Paged<PostDto>.PageFrom(posts.Select(p => p.ToDto(postService.ActingUser)),
            DTO.Posts.CreationDateComparer.Instance, paging);
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation("Create post")]
    public ActionResult<PostDto> CreatePost(CreatePost post)
    {
        Post newPost = postService.AsUser(User.GetGuid()).CreatePost(post.EventGuid, post.Content);
        return Ok(newPost.ToDto(postService.ActingUser));
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
    [Route("{id}/reactions")]
    [SwaggerOperation("List all post's reactions")]
    public ActionResult<IEnumerable<ReactionDto>> GetPostReactions([FromRoute] Guid id)
    {
        var reactions = postService.AsUser(User.GetGuid()).GetPost(id).Reactions;
        return Ok(reactions.Select(x => x.ToDto()));
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

    [HttpGet]
    [Authorize]
    [Route("{id}/comments")]
    [SwaggerOperation("List comments for given post")]
    public ActionResult<Paged<CommentDto>> GetPostComments([FromQuery] Paging paging, [FromRoute] Guid id)
    {
        var comments = postService.AsUser(User.GetGuid()).GetPost(id).Comments
            .Where(c => c.InResponeseToId is null);
        return Paged<CommentDto>.PageFrom(comments.Select(c => c.ToDto(postService.ActingUser)),
            DTO.Comments.CreationDateComparer.Instance, paging);
    }
}
