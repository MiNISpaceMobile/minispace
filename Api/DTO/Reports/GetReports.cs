using Domain.DataModel;

namespace Api.DTO.Reports;

public record GetReports(
    ICollection<ReportType> Types,
    bool Open = true,
    bool Closed = true,
    bool Ascending = false);
