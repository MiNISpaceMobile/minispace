using Api.DTO;
using Api.DTO.Users;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation("List users - admin only")]
    public ActionResult<Paged<PrivateUserDto>> GetUsers([FromQuery] Paging paging)
    {
        var users = userService.AsUser(User.GetGuid()).GetUsers();
        var paged = Paged<PrivateUserDto>.PageFrom(users.Select(u => u.ToPrivateDto()),
            UserNameComparer.Instance, paging);
        return Ok(paged);
    }

    [HttpGet]
    [Route("search")]
    [Authorize]
    [SwaggerOperation("List users with matching names")]
    public ActionResult<Paged<PublicUserDto>> GetSearchedUsers([FromQuery] Paging paging, [FromQuery] string firstName, [FromQuery] string lastName)
    {
        var users = userService.AsUser(User.GetGuid()).SearchUsers(firstName, lastName);
        var paged = Paged<PublicUserDto>.PageFrom(users.Select(x => x.ToDto()),
            UserNameComparer.Instance, paging);
        return Ok(paged);
    }

    [HttpGet]
    [Route("user")]
    [Authorize]
    [SwaggerOperation("Get acting user data")]
    public ActionResult<PrivateUserDto> Get()
    {
        var user = userService.AsUser(User.GetGuid()).GetUser();
        return Ok(user.ToPrivateDto());
    }

    [HttpPut]
    [Route("user")]
    [Authorize]
    [SwaggerOperation("Update acting user data")]
    public ActionResult<PrivateUserDto> Put([FromBody] UpdateUserDto dto)
    {
        var user = userService.AsUser(User.GetGuid()).UpdateUser(dto.FirstName, dto.LastName,
            dto.Email, dto.Description, dto.DateOfBirth, dto.EmailNotifications);
        return Ok(user.ToPrivateDto());
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
    [Route("user/friends")]
    [Authorize]
    [SwaggerOperation("Get acting user friend list")]
    public ActionResult<Paged<PublicUserDto>> GetFriends([FromQuery] Paging paging)
    {
        var friends = userService.AsUser(User.GetGuid()).GetUser().Friends;
        var paged = Paged<PublicUserDto>.PageFrom(friends.Select(x => x.ToDto()),
            UserNameComparer.Instance, paging);
        return Ok(paged);
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize]
    [SwaggerOperation("Get target user data - admin only")]
    public ActionResult<PrivateUserDto> Get([FromRoute] Guid id)
    {
        var user = userService.AsUser(User.GetGuid()).GetUser(id, true);
        return Ok(user.ToDto());
    }

    [HttpGet]
    [Route("{id}/public")]
    [Authorize]
    [SwaggerOperation("Get target user public data")]
    public ActionResult<PublicUserDto> GetPublic([FromRoute] Guid id)
    {
        var user = userService.AsUser(User.GetGuid()).GetUser(id, false);
        return Ok(user.ToDto());
    }

    [HttpPatch]
    [Route("{id}/roles")]
    [Authorize]
    [SwaggerOperation("Update target user roles - admin only")]
    public ActionResult PatchRoles([FromRoute] Guid id, [FromBody] SetUserRoles roles)
    {
        userService.AsUser(User.GetGuid()).UserRoles(id, roles.IsAdmin, roles.IsOrganizer);
        return Ok();
    }
}
