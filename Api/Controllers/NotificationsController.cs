using Api.DTO.Notifications;
using Api.DTO;
using Domain.DataModel;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Domain.Services.Implementations;

namespace Api.Controllers;

[ApiController]
public class NotificationsController : ControllerBase
{
    private IUserService userService;

    public NotificationsController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost]
    [Route("friend-requests")]
    [Authorize]
    [SwaggerOperation("Send a friend request")]
    public ActionResult PostFriendRequest([FromBody] SendFriendRequest sendFriendRequest)
    {
        userService.AsUser(User.GetGuid()).SendFriendRequest(sendFriendRequest.TargetUser);
        return Ok();
    }

    [HttpGet]
    [Route("friend-requests/sent")]
    [Authorize]
    [SwaggerOperation("List sent friend requests")]
    public ActionResult<Paged<FriendRequestDto>> GetSentFriendRequests([FromQuery] Paging paging)
    {
        var outgoing = userService.AsUser(User.GetGuid()).GetUser().SentFriendRequests;
        var paged = Paged<FriendRequestDto>.PageFrom(outgoing.Select((FriendRequest x) => x.ToDto(false)),
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
        var paged = Paged<FriendRequestDto>.PageFrom(outgoing.Select((FriendRequest x) => x.ToDto(true)),
            BaseNotificationTimestampComparer.Instance, paging);
        return Ok(paged);
    }

    [HttpPatch]
    [Route("friend-requests/{id}")]
    [Authorize]
    [SwaggerOperation("Accept or reject friend request")]
    public ActionResult PatchFriendRequest([FromRoute] Guid id, [FromBody] FriendResponseDto response)
    {
        userService.AsUser(User.GetGuid()).RespondFriendRequest(id, response.Accept);
        return Ok();
    }

    [HttpDelete]
    [Route("friend-requests/{id}")]
    [Authorize]
    [SwaggerOperation("Cancel friend request")]
    public ActionResult DeleteFriendRequest([FromRoute] Guid id)
    {
        userService.AsUser(User.GetGuid()).CancelFriendRequest(id);
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
    [Route("notifications/{id}")]
    [Authorize]
    [SwaggerOperation("Mark a notification as seen")]
    public ActionResult PatchNotification([FromRoute] Guid id)
    {
        userService.AsUser(User.GetGuid()).SeeNotification(id);
        return Ok();
    }

    // TODO: Add cyclic cleaning of seen notifications other than friend requests
}
