using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/pictures")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private IPictureService pictureService;

        public PicturesController(IPictureService pictureService)
        {
            this.pictureService = pictureService;
        }

        [HttpPost]
        [Route("user/profile")]
        [Produces("text/plain")]
        [Authorize]
        public ActionResult<string> PostUserProfilePicture(IFormFile picture)
        {
            using Stream s = picture.OpenReadStream();
            string url = pictureService.AsUser(User.GetGuid()).UploadUserProfilePicture(s);
            return Ok(url);
        }

        [HttpDelete]
        [Route("user/profile")]
        [Authorize]
        public ActionResult DeleteUserProfilePicture()
        {
            pictureService.AsUser(User.GetGuid()).DeleteUserProfilePicture();
            return Ok();
        }

        [HttpPost]
        [Route("event/{target}")]
        [Produces("text/plain")]
        [Authorize]
        public ActionResult<string> PostEventPicture([FromRoute] Guid target, IFormFile picture, [FromQuery] int index)
        {
            using Stream s = picture.OpenReadStream();
            string url = pictureService.AsUser(User.GetGuid()).UploadEventPicture(target, index, s);
            return Ok(url);
        }

        [HttpDelete]
        [Route("event/{target}")]
        [Produces("text/plain")]
        [Authorize]
        public ActionResult DeleteEventPicture([FromRoute] Guid target, [FromQuery] int index)
        {
            pictureService.AsUser(User.GetGuid()).DeleteEventPicture(target, index);
            return Ok();
        }

        [HttpPost]
        [Route("post/{target}")]
        [Produces("text/plain")]
        [Authorize]
        public ActionResult<string> PostPostPicture([FromRoute] Guid target, IFormFile picture, [FromQuery] int index)
        {
            using Stream s = picture.OpenReadStream();
            string url = pictureService.AsUser(User.GetGuid()).UploadPostPicture(target, index, s);
            return Ok(url);
        }

        [HttpDelete]
        [Route("post/{target}")]
        [Produces("text/plain")]
        [Authorize]
        public ActionResult DeletePostPicture([FromRoute] Guid target, [FromQuery] int index)
        {
            pictureService.AsUser(User.GetGuid()).DeletePostPicture(target, index);
            return Ok();
        }
    }
}
