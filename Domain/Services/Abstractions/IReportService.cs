using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IReportService : IBaseService<IReportService>
{
    Report CreateReport(Guid targetId, string title, string details, ReportCategory category, ReportType type);
    IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report;
    ReportType GetByGuid<ReportType>(Guid guid) where ReportType : Report;
    Report UpdateReport(Report newReport);
    void DeleteReport(Guid guid);
}
