using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IReportService : IBaseService<IReportService>
{
    ReportType CreateReport<TargetType, ReportType>(Guid targetId, Guid authorId, string title, string details, ReportCategory category)
        where TargetType : BaseEntity
        where ReportType : Report;
    IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report;
    ReportType GetByGuid<ReportType>(Guid guid) where ReportType : Report;
    Report UpdateReport(Report newReport);
    void DeleteReport(Guid guid);
}