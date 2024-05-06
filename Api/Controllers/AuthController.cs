using Api.Auth;
using Api.DTO.Auth;
using Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private IAuthenticator authenticator;
    private IJwtHandler jwtHandler;
    private ILogger logger;

    public AuthController(IAuthenticator authenticator, IJwtHandler jwtHandler, ILogger<AuthController> logger)
    {
        this.authenticator = authenticator;
        this.jwtHandler = jwtHandler;
        this.logger = logger;
    }

    [HttpPost]
    [Route("usos/requestToken")]
    [Route("usos/request_token")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<DTOTokenResponse> RequestToken([FromBody] DTOTokenRequest request)
    {
        try
        {
            (string token, string secret, string url) = authenticator.RequestAuthenticationToken(request.CallbackUrl);
            return Ok(new DTOTokenResponse(token, secret, url));
        }
        catch (Exception e)
        {
            logger.LogError($"Token request failed: {e.Message}");
            return this.FailedDependency();
        }
    }

    [HttpPost]
    [Route("usos/jwt")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<DTOJwtResponse> RequestJwt([FromBody] DTOJwtRequest request)
    {
        try
        {
            string jwt = jwtHandler.Encode(authenticator.Authenticate(request.Token, request.Secret, request.Verifier));
            return Ok(new DTOJwtResponse(jwt));
        }
        catch (Exception e)
        {
            logger.LogError($"JWT request failed: {e.Message}");
            return this.FailedDependency();
        }
    }

    [HttpGet]
    [Route("jwt/info")]
    [Route("usos/jwt/info")]
    [Produces("application/json")]
    public ActionResult<DTOJwtInfo> GetJwtInfo([FromHeader] string authorization)
    {
        try
        {
            string token = IJwtHandler.StripAuthSchemeName(authorization, JwtAuthScheme.SchemeType);
            (Guid guid, bool expired) = jwtHandler.DecodeEvenIfExpired(token)!.Value;
            DTOJwtInfo info = new DTOJwtInfo(guid, expired);
            return Ok(info);
        }
        catch
        {
            return BadRequest("No valid JWT was provided");
        }
    }
}
