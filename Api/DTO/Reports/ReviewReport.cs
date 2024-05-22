
using Domain.DataModel;

namespace Api.DTO.Reports;

public record ReviewReport(
    string? Feedback,
    ReportState State);
