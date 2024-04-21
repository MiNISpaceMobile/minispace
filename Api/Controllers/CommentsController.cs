using Api.DTO.Comments;
using Api.DTO.Paging;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// TODO: Implement controller
[Route("comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    /// <summary>
    /// Creates a comment. There should be exactly one of postId or eventId fields present.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult AddComment(CreateComment createComment)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes a comment with given id. Users can delete only their own comments unless they have the ADMIN role.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DeleteComment(long id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Searches for comments corresponding to given event or post. There should be exactly one of postId or eventId fields present.
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType<PagedResponse<CommentDto>>(StatusCodes.Status200OK)]
    public ActionResult<PagedResponse<CommentDto>> SearchComments(CommentSearchDetails details)
    {
        throw new NotImplementedException();
    }
}