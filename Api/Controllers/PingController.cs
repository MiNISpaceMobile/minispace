using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("ping")]
[ApiController]
public class PingController : ControllerBase
{
    private IPingResponder pingResponder;

    public PingController(IPingResponder pingResponder)
    {
        this.pingResponder = pingResponder;
    }

    [HttpGet]
    [Produces("text/plain")]
    public ActionResult<string> GetPing()
    {
        return Ok(pingResponder.Response());
    }
}
