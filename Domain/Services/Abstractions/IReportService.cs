using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IReportService : IBaseService<IReportService>
{
    Report CreateReport(Guid targetId, string title, string details, ReportType type);
    IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report;
    IEnumerable<Report> GetReports(ICollection<ReportType> types, bool open, bool closed);
    ReportType GetByGuid<ReportType>(Guid guid) where ReportType : Report;
    Report GetByGuid(Guid guid);
    Report ReviewReport(Guid reportGuid, string? feedback);
    void DeleteReport(Guid guid);
}
