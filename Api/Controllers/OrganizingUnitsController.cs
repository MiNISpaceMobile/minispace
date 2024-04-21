using Api.DTO.OrganizingUnits;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// TODO: Implement controller
[Route("organizingUnits")]
[ApiController]
public class OrganizingUnitsController : ControllerBase
{
    /// <summary>
    /// Create a new organizing unit with given parameters.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateUnit(OrganizingUnitDto organizingUnit)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get organizing units of a user
    /// If the userId parameter is not present - get all organizing units a user belongs
    /// to determined by userId embedded in user's acces token.
    /// </summary>
    [HttpPost("ofUser")]
    [ProducesResponseType<IEnumerable<OrganizingUnitDto>>(StatusCodes.Status200OK)]
    public IActionResult GetUnitsOfUser(long? userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add a user to an organizing unit.
    /// </summary>
    [HttpPost("{id}/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult AddUserToUnit(int id, int userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Delete a user from an organizing unit.
    /// </summary>
    [HttpDelete("{id}/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DeleteUserFromUnit(int id, int userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all organizing units which do not have a parent.
    /// </summary>
    [HttpGet("roots")]
    [ProducesResponseType<IEnumerable<OrganizingUnitSearchResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<OrganizingUnitSearchResponse>> GetRoots()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all organizing units which have a parent referenced by parentId.
    /// </summary>
    [HttpGet("{id}/children")]
    [ProducesResponseType<IEnumerable<OrganizingUnitSearchResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<OrganizingUnitSearchResponse>> GetChildren(int id)
    {
        throw new NotImplementedException();
    }
}
