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
    private ILogger logger;

    public AuthController(UsosAuthentication usos, ILogger<AuthController> logger)
    {
        this.usos = usos;
        this.logger = logger;
    }

    [HttpPost]
    [Route("usos/requestToken")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<DTOLoginResponse> RequestToken([FromBody] DTOLoginRequest request)
    {
        try
        {
            return Ok(usos.RequestLogin(request));
        }
        catch (Exception e)
        {
            logger.LogError($"Login request failed: {e.Message}");
            return this.FailedDependency();
        }
    }

    [HttpPost]
    [Route("usos/jwt")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<DTOAccessResponse> RequestJWT([FromBody] DTOAccessRequest request)
    {
        try
        {
            return Ok(usos.RequestAccess(request));
        }
        catch (Exception e)
        {
            logger.LogError($"Access request failed: {e.Message}");
            return this.FailedDependency();
        }
    }

    // TODO: Add endpoint for checking JWT validity
}
