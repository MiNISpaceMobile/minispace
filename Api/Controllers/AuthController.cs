using Api.Auth;
using Api.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private UsosAuthentication usos;

    public AuthController(UsosAuthentication usos)
    {
        this.usos = usos;
    }

    // TODO: Proper error/exception handling

    [HttpPost]
    [Route("usos/requestToken")]
    [Consumes("application/json")]
    [Produces("apllication/json")]
    public ActionResult<DTOLoginResponse> RequestToken([FromBody] DTOLoginRequest request)
    {
        return Ok(usos.RequestLogin(request));
    }

    [HttpPost]
    [Route("usos/jwt")]
    [Consumes("application/json")]
    [Produces("apllication/json")]
    public ActionResult<DTOAccessResponse> RequestJWT([FromBody] DTOAccessRequest request)
    {
        return Ok(usos.RequestAccess(request));
    }
}
