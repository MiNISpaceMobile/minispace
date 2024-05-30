using Api.DTO;
using Api.DTO.Comments;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("comments")]
[ApiController]
public class CommentController : ControllerBase
{
    private ICommentService commentService;

    public CommentController(ICommentService commentService)
    {
        this.commentService = commentService;
    }

    [HttpGet]
    [Authorize]
    [Route("responses")]
    [SwaggerOperation("List comment's responses")]
    public ActionResult<Paged<CommentDto>> GetCommentResponses([FromQuery] Paging paging, Guid commentGuid)
    {
        var comments = commentService.AsUser(User.GetGuid()).GetComment(commentGuid).Responses;
        return Paged<CommentDto>.PageFrom(comments.AsEnumerable().Select(p => p.ToDto(commentService.ActingUser)),
            CreationDateComparer.Instance, paging);
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation("Create comment")]
    public ActionResult<CommentDto> CreateComment([FromBody] CreateComment newComment)
    {
        Guid inResponseTo = new Guid();
        if (newComment.InResponseTo is not null)
            inResponseTo = (Guid)newComment.InResponseTo;
        var comment = commentService.AsUser(User.GetGuid()).CreateComment(newComment.PostGuid, newComment.Content, inResponseTo, DateTime.Now);
        return Ok(comment.ToDto(commentService.ActingUser));
    }

    [HttpDelete]
    [Authorize]
    [Route("{id}")]
    [SwaggerOperation("Delete comment")]
    public ActionResult DeleteComment([FromRoute] Guid id)
    {
        commentService.AsUser(User.GetGuid()).DeleteComment(id);
        return Ok();
    }
}
