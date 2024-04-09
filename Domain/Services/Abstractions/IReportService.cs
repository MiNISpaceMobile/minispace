using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services
{
    public interface IReportService
    {
        ReportType CreateReport<TargetType, ReportType>(Guid targetId, Guid authorId, string title, string details, ReportCategory category)
            where TargetType : BaseEntity
            where ReportType : Report;
        IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report;
        ReportType GetByGuid<ReportType>(Guid guid) where ReportType : Report;
        Report UpdateReport(Guid responderId, Guid reportId, string feedback, ReportState reportState);
    }
}