using Api.DTO;
using Api.DTO.Reports;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("reports")]
[ApiController]
[Authorize]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    public ActionResult<ReportDto> CreateReport([FromBody] CreateReport request)
    {
        try
        {
            var report = reportService
                .AsUser(User.GetGuid())
                .CreateReport(request.TargetId, request.Title, request.Details,
                request.ReportCategory, request.ReportType)
                .ToDto();

            return Ok(report);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Produces("application/json")]
    public ActionResult<IEnumerable<ReportDto>> GetReports([FromQuery] GetReports request)
    {
        try
        {
            var reports = reportService
                .AsUser(User.GetGuid())
                .GetReports(request.Types, request.Open, request.Closed, request.Ascending,
                request.PageIndex, request.PageSize)
                .Map(report => report.ToDto());

            return Ok(reports);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch("{id}")]
    [Produces("application/json")]
    public ActionResult<ReportDto> ReviewReport([FromRoute] Guid id, [FromBody] ReviewReport request)
    {
        try
        {
            var report = reportService
                .AsUser(User.GetGuid())
                .ReviewReport(id, request.Feedback, request.State)
                .ToDto();

            return Ok(report);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteReport([FromRoute] Guid id)
    {
        try
        {
            reportService
                .AsUser(User.GetGuid())
                .DeleteReport(id);

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
