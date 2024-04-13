using Domain.Abstractions;
using Domain.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// *Don't* leave that for production
#if DEBUG

[Route("test")]
[ApiController]
public class TestController : ControllerBase
{
    private IUnitOfWork uow;

    public TestController(IUnitOfWork uow, IHostEnvironment environment)
    {
        // *Don't* leave that for production
        if (!environment.IsDevelopment())
            throw new Exception();
        this.uow = uow;
    }

    // TODO: Add DTOs

    [HttpGet]
    [Route("users")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(uow.Repository<User>().GetAll());
    }

    [HttpGet]
    [Route("admins")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Administrator>> GetAdmins()
    {
        return Ok(uow.Repository<Administrator>().GetAll());
    }

    [HttpGet]
    [Route("students")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Student>> GetStudents()
    {
        var s = uow.Repository<Student>().GetAll().ToArray();
        return Ok(s);
    }

    [HttpGet]
    [Route("events")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Event>> GetEvents()
    {
        return Ok(uow.Repository<Event>().GetAll());
    }

    [HttpGet]
    [Route("posts")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Post>> GetPosts()
    {
        return Ok(uow.Repository<Post>().GetAll());
    }

    [HttpGet]
    [Route("comments")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Comment>> GetComments()
    {
        return Ok(uow.Repository<Comment>().GetAll());
    }

    [HttpGet]
    [Route("reports")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Report>> GetReports()
    {
        return Ok(uow.Repository<Report>().GetAll());
    }

    [HttpGet]
    [Route("event_reports")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<EventReport>> GetEventReports()
    {
        return Ok(uow.Repository<EventReport>().GetAll());
    }

    [HttpGet]
    [Route("post_reports")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<PostReport>> GetPostReports()
    {
        return Ok(uow.Repository<PostReport>().GetAll());
    }

    [HttpGet]
    [Route("comment_reports")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<CommentReport>> GetCommentReports()
    {
        return Ok(uow.Repository<CommentReport>().GetAll());
    }
}

#endif
