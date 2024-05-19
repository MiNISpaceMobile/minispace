using Api.DTO;
using Api.DTO.Users;
using Domain.Services;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("search")]
        public ActionResult<Paged<UserDto>> Search([FromBody] Paging paging)
        {
            try
            {
                var users = userService.AsUser(User.GetGuid()).GetUsers();
                var paged = Paged<UserDto>.PageFrom(users.Select(u => u.ToDto()), UserNameComparer.Instance, paging);
                return Ok(paged);
            }
            catch (UserUnauthorizedException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // TODO: Roles
    }
}
