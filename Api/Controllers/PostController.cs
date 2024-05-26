using Api.DTO;
using Api.DTO.Events;
using Api.DTO.Posts;
using Api.DTO.Users;
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
    [SwaggerOperation("List user's posts")]
    public ActionResult<Paged<PostDto>> GetUsersPosts([FromQuery] Paging paging, Guid userGuid, bool showAlsoInterested)
    {
        var posts = postService.AsUser(User.GetGuid()).GetUsersPosts(userGuid);
        if (!showAlsoInterested)
            posts = posts.FindAll(p => p.Event.Participants.FirstOrDefault(part => part.Guid == userGuid) is not null);
        return Paged<PostDto>.PageFrom(posts.Select(p => p.ToDto()), CreationDateComparer.Instance, paging);
    }
}
