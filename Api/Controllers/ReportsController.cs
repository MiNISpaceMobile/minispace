using Api.DTO;
using Api.DTO.Reports;
using Domain.Services.Abstractions;
using Domain.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("reports")]
[ApiController]
[Authorize]
public class ReportsController(IReportService reportService) : ControllerBase
{
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
    public ActionResult<Paged<ReportDto>> GetByGuid([FromRoute] Guid id)
    {
        var report = reportService
            .AsUser(User.GetGuid())
            .GetByGuid(id)
            .ToDto();

        return Ok(report);
    }

    [HttpGet]
    [SwaggerOperation("Get reports - user gets only his reports, admin gets all reports")]
    public ActionResult<Paged<ReportDto>> GetReports([FromQuery] GetReports request, [FromQuery] Paging paging)
    {
        var reports = reportService
            .AsUser(User.GetGuid())
            .GetReports(request.Types, request.Open, request.Closed)
            .Select(report => report.ToDto());
        var comparer = ReportUpdateDateComparer.Instance(request.Ascending);
        var paged = Paged<ReportDto>.PageFrom(reports, comparer, paging);

        return Ok(paged);
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

        return NoContent();
    }
}
