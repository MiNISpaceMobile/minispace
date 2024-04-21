using Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// TODO: Implement controller
[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Assigns a role to the user with given id.
    /// </summary>
    [HttpPost("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult AssignRole(int id, string role)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a role from the user referenced by id.
    /// </summary>
    [HttpDelete("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult RemoveRole(int id, string role)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Searches for users with provided filters and sorting, returns a single pageable object of users.
    /// Available only for admin.
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType<PagedResponse<UserDto>>(StatusCodes.Status200OK)]
    public ActionResult<PagedResponse<UserDto>> SearchUsers(UserSearchDetails details)
    {
        throw new NotImplementedException();
    }
}