using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        [Produces("text/plain")]
        public ActionResult<string> GetPing() => Ok("Pong");

    }
}
