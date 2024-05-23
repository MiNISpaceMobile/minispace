using Api.DTO;
using Api.DTO.Notifications;
using Api.DTO.Users;
using Domain.DataModel;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [Route("users")]
        [Authorize]
        [SwaggerOperation("List users - admin only")]
        public ActionResult<Paged<UserDto>> GetUsers([FromQuery] Paging paging)
        {
            var users = userService.AsUser(User.GetGuid()).GetUsers();
            var paged = Paged<UserDto>.PageFrom(users.Select(u => u.ToDto()),
                UserNameComparer.Instance, paging);
            return Ok(paged);
        }

        [HttpGet]
        [Route("user")]
        [Authorize]
        [SwaggerOperation("Get acting user data")]
        public ActionResult<UserDto> Get()
        {
            var user = userService.AsUser(User.GetGuid()).GetUser();
            return Ok(user.ToDto());
        }

        [HttpPut]
        [Route("user")]
        [Authorize]
        [SwaggerOperation("Update acting user data")]
        public ActionResult<UserDto> Put([FromBody] UpdateUserDto dto)
        {
            var user = userService.AsUser(User.GetGuid()).UpdateUser(dto.FirstName, dto.LastName,
                dto.Email, dto.Description, dto.DateOfBirth, dto.EmailNotifications);
            return Ok(user.ToDto());
        }

        [HttpDelete]
        [Route("user")]
        [Authorize]
        [SwaggerOperation("Delete acting user account")]
        public ActionResult Delete()
        {
            userService.AsUser(User.GetGuid()).DeleteUser();
            return Ok();
        }

        [HttpGet]
        [Route("user/{target}")]
        [Authorize]
        [SwaggerOperation("Get target user data - admin only")]
        public ActionResult<UserDto> Get([FromRoute] Guid target)
        {
            var user = userService.AsUser(User.GetGuid()).GetUser(target);
            return Ok(user.ToDto());
        }

        [HttpPatch]
        [Route("user/{target}/roles")]
        [Authorize]
        [SwaggerOperation("Update target user roles - admin only")]
        public ActionResult<(bool IsAdmin, bool IsOrganizer)> Patch([FromRoute] Guid target, [FromQuery] bool? isAdmin, [FromQuery] bool? isOrganizer)
        {
            var roles = userService.AsUser(User.GetGuid()).UserRoles(target, isAdmin, isOrganizer);
            return Ok(roles);
        }

        [HttpPost]
        [Route("friend-requests")]
        [Authorize]
        [SwaggerOperation("Send a friend request")]
        public ActionResult PostFriendRequest([FromQuery] Guid targetUser)
        {
            userService.AsUser(User.GetGuid()).SendFriendRequest(targetUser);
            return Ok();
        }

        [HttpGet]
        [Route("friend-requests/sent")]
        [Authorize]
        [SwaggerOperation("List sent friend requests")]
        public ActionResult<Paged<FriendRequestDto>> GetSentFriendRequests([FromQuery] Paging paging)
        {
            var outgoing = userService.AsUser(User.GetGuid()).GetUser().SentFriendRequests;
            var paged = Paged<FriendRequestDto>.PageFrom(outgoing.Select((FriendRequest x) => x.ToDto()),
                BaseNotificationTimestampComparer.Instance, paging);
            return Ok(paged);
        }

        [HttpGet]
        [Route("friend-requests/received")]
        [Authorize]
        [SwaggerOperation("List received friend requests")]
        public ActionResult<Paged<FriendRequestDto>> GetReceivedFriendRequests([FromQuery] Paging paging)
        {
            var outgoing = userService.AsUser(User.GetGuid()).GetUser().ReceivedFriendRequests;
            var paged = Paged<FriendRequestDto>.PageFrom(outgoing.Select((FriendRequest x) => x.ToDto()),
                BaseNotificationTimestampComparer.Instance, paging);
            return Ok(paged);
        }

        [HttpPatch]
        [Route("friend-request/{target}")]
        [Authorize]
        [SwaggerOperation("Accept / reject friend request")]
        public ActionResult PatchFriendRequest([FromRoute] Guid target, [FromQuery] bool accept)
        {
            userService.AsUser(User.GetGuid()).RespondFriendRequest(target, accept);
            return Ok();
        }

        [HttpDelete]
        [Route("friend-request/{target}")]
        [Authorize]
        [SwaggerOperation("Cancel friend request")]
        public ActionResult DeleteFriendRequest([FromRoute] Guid target)
        {
            userService.AsUser(User.GetGuid()).CancelFriendRequest(target);
            return Ok();
        }

        [HttpGet]
        [Route("notifications")]
        [Authorize]
        [SwaggerOperation("List received notifications")]
        public ActionResult<Paged<NotificationDto>> GetNotifications([FromQuery] Paging paging)
        {
            var notifications = userService.AsUser(User.GetGuid()).GetNotifications();
            var paged = Paged<NotificationDto>.PageFrom(notifications.Select(x => x.ToDto()),
                BaseNotificationTimestampComparer.Instance, paging);
            return Ok(paged);
        }

        [HttpPatch]
        [Route("notifications")]
        [Authorize]
        [SwaggerOperation("Mark all notifications as seen")]
        public ActionResult PatchNotifications()
        {
            userService.AsUser(User.GetGuid()).SeeAllNotifications();
            return Ok();
        }

        [HttpPatch]
        [Route("notification/{target}")]
        [Authorize]
        [SwaggerOperation("Mark a notification as seen")]
        public ActionResult PatchNotification([FromRoute] Guid target)
        {
            userService.AsUser(User.GetGuid()).SeeNotification(target);
            return Ok();
        }

        // TODO: Add cyclic cleaning of seen notifications other than friend requests
    }
}
