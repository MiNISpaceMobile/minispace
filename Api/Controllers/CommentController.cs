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
        return Paged<CommentDto>.PageFrom(comments.AsEnumerable().Select(p => p.ToDto()), DTO.Comments.CreationDateComparer.Instance, paging);
    }
}
