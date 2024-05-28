using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class PicturesController : ControllerBase
{
    private IPictureService pictureService;

    public PicturesController(IPictureService pictureService)
    {
        this.pictureService = pictureService;
    }

    [HttpPost]
    [Route("users/user/picture")]
    [Produces("text/plain")]
    [Authorize]
    public ActionResult<string> PostUserProfilePicture(IFormFile picture)
    {
        using Stream s = picture.OpenReadStream();
        string url = pictureService.AsUser(User.GetGuid()).UploadUserProfilePicture(s);
        return Ok(url);
    }

    [HttpDelete]
    [Route("users/user/picture")]
    [Authorize]
    public ActionResult DeleteUserProfilePicture()
    {
        pictureService.AsUser(User.GetGuid()).DeleteUserProfilePicture();
        return Ok();
    }

    [HttpPost]
    [Route("events/{id}/pictures")]
    [Produces("text/plain")]
    [Authorize]
    public ActionResult<string> PostEventPicture([FromRoute] Guid id, IFormFile picture, [FromQuery] int index)
    {
        using Stream s = picture.OpenReadStream();
        string url = pictureService.AsUser(User.GetGuid()).UploadEventPicture(id, index, s);
        return Ok(url);
    }

    [HttpDelete]
    [Route("events/{id}/pictures")]
    [Authorize]
    public ActionResult DeleteEventPicture([FromRoute] Guid id, [FromQuery] int index)
    {
        pictureService.AsUser(User.GetGuid()).DeleteEventPicture(id, index);
        return Ok();
    }

    [HttpPost]
    [Route("posts/{id}/pictures")]
    [Produces("text/plain")]
    [Authorize]
    public ActionResult<string> PostPostPicture([FromRoute] Guid id, IFormFile picture, [FromQuery] int index)
    {
        using Stream s = picture.OpenReadStream();
        string url = pictureService.AsUser(User.GetGuid()).UploadPostPicture(id, index, s);
        return Ok(url);
    }

    [HttpDelete]
    [Route("posts/{id}/pictures")]
    [Authorize]
    public ActionResult DeletePostPicture([FromRoute] Guid id, [FromQuery] int index)
    {
        pictureService.AsUser(User.GetGuid()).DeletePostPicture(id, index);
        return Ok();
    }
}
