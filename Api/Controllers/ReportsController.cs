using Api.DTO;
using Api.DTO.Reports;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("reports")]
[ApiController]
[Authorize]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("List all reports - admin only")]
    public ActionResult<Paged<ReportDto>> GetReports([FromQuery] GetReports request, [FromQuery] Paging paging)
    {
        var reports = reportService
            .AsUser(User.GetGuid())
            .GetReports(request.Types, request.Open, request.Closed, true)
            .Select(report => report.ToDto());
        var comparer = ReportUpdateDateComparer.Instance(request.Ascending);
        var paged = Paged<ReportDto>.PageFrom(reports, comparer, paging);

        return Ok(paged);
    }

    [HttpGet]
    [Route("user")]
    [SwaggerOperation("List reports created by acting user")]
    public ActionResult<Paged<ReportDto>> GetUserReports([FromQuery] GetReports request, [FromQuery] Paging paging)
    {
        var reports = reportService
            .AsUser(User.GetGuid())
            .GetReports(request.Types, request.Open, request.Closed, false)
            .Select(report => report.ToDto());
        var comparer = ReportUpdateDateComparer.Instance(request.Ascending);
        var paged = Paged<ReportDto>.PageFrom(reports, comparer, paging);

        return Ok(paged);
    }

    [HttpPost]
    [SwaggerOperation("Create report")]
    public ActionResult<ReportDto> CreateReport([FromBody] CreateReport request)
    {
        var report = reportService
            .AsUser(User.GetGuid())
            .CreateReport(request.TargetId, request.Title, request.Details, request.ReportType)
            .ToDto();

        return Ok(report);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get report by id")]
    public ActionResult<ReportDto> GetByGuid([FromRoute] Guid id)
    {
        var report = reportService
            .AsUser(User.GetGuid())
            .GetByGuid(id)
            .ToDto();

        return Ok(report);
    }

    [HttpPatch("{id}")]
    [SwaggerOperation("Review report - admin only")]
    public ActionResult<ReportDto> ReviewReport([FromRoute] Guid id, [FromBody] ReviewReport request)
    {
        var report = reportService
            .AsUser(User.GetGuid())
            .ReviewReport(id, request.Feedback)
            .ToDto();

        return Ok(report);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Delete report")]
    public ActionResult DeleteReport([FromRoute] Guid id)
    {
        reportService
            .AsUser(User.GetGuid())
            .DeleteReport(id);

        return Ok();
    }
}
