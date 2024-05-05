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
    private JwtService jwtService;
    private ILogger logger;

    public AuthController(UsosAuthentication usos, JwtService jwtService, ILogger<AuthController> logger)
    {
        this.usos = usos;
        this.jwtService = jwtService;
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
    public ActionResult<DTOAccessResponse> RequestJwt([FromBody] DTOAccessRequest request)
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

    [HttpGet]
    [Route("usos/jwt/info")]
    [Produces("application/json")]
    public ActionResult<DTOJwtInfo> GetJwtInfo([FromHeader] string authorization)
    {
        try
        {
            string token = jwtService.StripAuthSchemeName(authorization, JwtAuthScheme.SchemeType);
            (Guid guid, bool expired) = jwtService.DecodeEvenIfExpired(token)!.Value;
            DTOJwtInfo info = new DTOJwtInfo(guid, expired);
            return Ok(info);
        }
        catch
        {
            return BadRequest("No valid JWT was provided");
        }
    }
}
