using Api.DTO;
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
            posts = posts.FindAll(p => p.Event.Participants.FirstOrDefault(part => part.Guid == User.GetGuid()) is not null);
        return Paged<PostDto>.PageFrom(posts.Select(p => p.ToDto()), CreationDateComparer.Instance, paging);
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation("Create post")]
    public ActionResult<PostDto> CreatePost(CreatePost post)
    {
        Post newPost = postService.AsUser(User.GetGuid()).CreatePost(post.EventGuid, post.Content);
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
