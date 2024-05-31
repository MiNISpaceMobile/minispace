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
    [Route("{id}/responses")]
    [SwaggerOperation("List comment's responses")]
    public ActionResult<Paged<CommentDto>> GetCommentResponses([FromQuery] Paging paging, [FromRoute] Guid id)
    {
        var comments = commentService.AsUser(User.GetGuid()).GetComment(id).Responses;
        return Paged<CommentDto>.PageFrom(comments.Select(p => p.ToDto(User.GetGuid())),
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
        return Ok(comment.ToDto(User.GetGuid()));
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

    [HttpGet]
    [Authorize]
    [Route("{id}/likes")]
    [SwaggerOperation("List all comments's likes and dislikes")]
    public ActionResult<Paged<LikeDto>> GetCommentLikes([FromRoute] Guid id, [FromQuery] Paging paging)
    {
        var likes = commentService.AsUser(User.GetGuid()).GetComment(id).Likes;
        var paged = Paged<LikeDto>.PageFrom(likes.Select(x => x.ToDto(commentService.ActingUser!)),
            ByFriendComparer.Instance, paging);
        return Ok(paged);
    }

    [HttpPatch]
    [Authorize]
    [Route("{id}/likes")]
    [SwaggerOperation("Set acting user's like/dislike to comment")]
    public ActionResult PatchLike([FromRoute] Guid id, [FromBody] SetLike like)
    {
        commentService.AsUser(User.GetGuid()).SetLike(id, like.IsDislike);
        return Ok();
    }
}
