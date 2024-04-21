using Api.DTO.Posts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// TODO: Implement controller
[Route("posts")]
[ApiController]
public class PostsController : ControllerBase
{
    /// <summary>
    /// Get all posts which correspond to the event with given eventId.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PostDto>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<PostDto>> GetCorrespondingPosts(long eventId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Register new post.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    public ActionResult<PostDto> AddPost(RegisterPost request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get post corresponding to provided id.
    /// </summary>
    [HttpGet("{postId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    public ActionResult<PostDto> GetPost(long postId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Delete post corresponding to provided id.
    /// </summary>
    [HttpDelete("{postId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DeletePost(long postId)
    {
        throw new NotImplementedException();
    }
}
