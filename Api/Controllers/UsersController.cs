using Api.DTO;
using Api.DTO.Notifications;
using Api.DTO.Users;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("users/search")]
        [Authorize]
        public ActionResult<Paged<UserDto>> GetUsers([FromQuery] Paging paging)
        {
            var users = userService.AsUser(User.GetGuid()).GetUsers();
            var paged = Paged<UserDto>.PageFrom(users.Select(u => u.ToDto()), UserNameComparer.Instance, paging);
            return Ok(paged);
        }

        [HttpGet]
        [Route("user/{target}")]
        [Authorize]
        public ActionResult<UserDto> Get([FromRoute] Guid target)
        {
            var user = userService.AsUser(User.GetGuid()).GetUser(target);
            return Ok(user.ToDto());
        }

        [HttpGet]
        [Route("user")]
        [Authorize]
        public ActionResult<UserDto> Get()
        {
            var user = userService.AsUser(User.GetGuid()).GetUser();
            return Ok(user.ToDto());
        }

        [HttpPut]
        [Route("user")]
        [Authorize]
        public ActionResult<UserDto> Put([FromBody] UpdateUserDto dto)
        {
            var user = userService.AsUser(User.GetGuid()).UpdateUser(dto.FirstName, dto.LastName,
                dto.Email, dto.Description, dto.DateOfBirth, dto.EmailNotifications);
            return Ok(user.ToDto());
        }

        [HttpDelete]
        [Route("user")]
        [Authorize]
        public ActionResult Delete()
        {
            userService.AsUser(User.GetGuid()).DeleteUser();
            return Ok();
        }

        [HttpPatch]
        [Route("user/{target}/roles")]
        [Authorize]
        public ActionResult<(bool IsAdmin, bool IsOrganizer)> Patch([FromRoute] Guid target, [FromQuery] bool? isAdmin, [FromQuery] bool? isOrganizer)
        {
            var roles = userService.AsUser(User.GetGuid()).UserRoles(target, isAdmin, isOrganizer);
            return Ok(roles);
        }

        [HttpPost]
        [Route("friend-requests")]
        [Authorize]
        public ActionResult PostFriendRequest([FromQuery] Guid targetUser)
        {
            userService.AsUser(User.GetGuid()).SendFriendRequest(targetUser);
            return Ok();
        }

        [HttpPatch]
        [Route("friend-request/{target}")]
        [Authorize]
        public ActionResult PatchFriendRequest([FromRoute] Guid target, [FromQuery] bool accept)
        {
            userService.AsUser(User.GetGuid()).RespondFriendRequest(target, accept);
            return Ok();
        }

        [HttpGet]
        [Route("user/notifications")]
        [Authorize]
        public ActionResult<Paged<NotificationDto>> GetNotifications([FromQuery] Paging paging)
        {
            var notifications = userService.AsUser(User.GetGuid()).GetNotifications();
            var paged = Paged<NotificationDto>.PageFrom(notifications.Select(x => x.ToDto()), NotificationTimestampComparer.Instance, paging);
            return Ok(paged);
        }

        [HttpPatch]
        [Route("user/notifications")]
        [Authorize]
        public ActionResult PatchNotifications()
        {
            userService.AsUser(User.GetGuid()).SeeAllNotifications();
            return Ok();
        }

        [HttpPatch]
        [Route("notification/{target}")]
        [Authorize]
        public ActionResult PatchNotification([FromRoute] Guid target)
        {
            userService.AsUser(User.GetGuid()).SeeNotification(target);
            return Ok();
        }

        // TODO: Add cyclic cleaning of seen notifications 
    }
}
