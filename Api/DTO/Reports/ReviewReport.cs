
using Domain.DataModel;

namespace Api.DTO.Reports;

public record ReviewReport(
    Guid ReportGuid,
    string? Feedback,
    ReportState State);
