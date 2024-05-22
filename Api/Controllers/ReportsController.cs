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
            return reportService
                .AsUser(User.GetGuid())
                .CreateReport(request.TargetId, request.Title, request.Details,
                request.ReportCategory, request.ReportType)
                .ToDto();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
