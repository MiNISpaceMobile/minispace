using Api.DTO;
using Api.DTO.Comments;
using Api.DTO.Events;
using Api.DTO.Posts;
using Api.DTO.Users;
using Domain.DataModel;
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
    private IPostService postService;

    public CommentController(ICommentService commentService, IPostService postService)
    {
        this.commentService = commentService;
        this.postService = postService;
    }

    [HttpGet]
    [Authorize]
    [Route("post")]
    [SwaggerOperation("List comments for given post")]
    public ActionResult<Paged<CommentDto>> GetPostComments([FromQuery] Paging paging, Guid postGuid)
    {
        var comments = postService.AsUser(User.GetGuid()).GetPost(postGuid).Comments;
        return Paged<CommentDto>.PageFrom(comments.AsEnumerable().Where(c => c.InResponeseToId is null).Select(c => c.ToDto()), DTO.Comments.CreationDateComparer.Instance, paging);
    }

    [HttpGet]
    [Authorize]
    [Route("responses")]
    [SwaggerOperation("List comment's responses")]
    public ActionResult<Paged<CommentDto>> GetCommentResponses([FromQuery] Paging paging, Guid commentGuid)
    {
        var comments = commentService.AsUser(User.GetGuid()).GetComment(commentGuid).Responses;
        return Paged<CommentDto>.PageFrom(comments.AsEnumerable().Select(p => p.ToDto()), DTO.Comments.CreationDateComparer.Instance, paging);
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
        return Ok(comment.ToDto());
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
