using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IReportService : IBaseService<IReportService>
{
    Report CreateReport(Guid targetId, string title, string details, ReportCategory category, ReportType type);
    IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report;
    IEnumerable<Report> GetReports(ICollection<ReportType> types, bool open, bool closed, bool ascending);
    ReportType GetByGuid<ReportType>(Guid guid) where ReportType : Report;
    Report ReviewReport(Guid reportGuid, string? feedback, ReportState state);
    void DeleteReport(Guid guid);
}
