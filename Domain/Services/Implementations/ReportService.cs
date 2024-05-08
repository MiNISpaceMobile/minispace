using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork uow) : BaseService<IReportService, ReportService>(uow), IReportService
{
    // TODO: Change all exception to custom exceptions with proper error information
    public IEnumerable<ReportType> GetAll<ReportType>()
        where ReportType : Report
    {
        return uow.Repository<ReportType>().GetAll();
    }

    public ReportType GetByGuid<ReportType>(Guid guid)
        where ReportType : Report
    {
        var report = uow.Repository<ReportType>().Get(guid);
        return report is not null ? report : throw new Exception("Invalid report guid");
    }

    public ReportType CreateReport<TargetType, ReportType>(Guid targetId, Guid authorId, string title,
        string details, ReportCategory category)
        where TargetType : BaseEntity
        where ReportType : Report
    {
        var target = uow.Repository<TargetType>().Get(targetId) ??
            throw new Exception("Invalid target guid");
        var author = uow.Repository<Student>().Get(authorId) ??
            throw new Exception("Invalid author guid");

        var report = (ReportType)CreateSpecificReport(target, author, title, details, category);

        uow.Repository<ReportType>().Add(report);

        uow.Commit();

        return report;
    }

    public Report UpdateReport(Report newReport)
    {
        var responderId = newReport.ResponderId ??
            throw new Exception("No responder guid");
        var responder = uow.Repository<Administrator>().Get(responderId) ??
            throw new Exception("Invalid responder guid");
        var report = uow.Repository<Report>().Get(newReport.Guid) ??
            throw new Exception("Invalid report guid");
        if (!report.IsOpen)
            throw new Exception("Report is closed");

        report.Responder = responder;
        report.Feedback = newReport.Feedback;
        report.State = newReport.State;

        uow.Commit();

        return report;
    }

    public void DeleteReport(Guid guid)
    {
        if (!uow.Repository<Report>().TryDelete(guid))
            throw new Exception("Invalid guid");

        uow.Commit();
    }

    private static Report CreateSpecificReport<TargetType>(TargetType target, User author, string title,
        string details, ReportCategory category)
        where TargetType : BaseEntity
    {
        return target switch
        {
            Event @event => new EventReport(@event, author, title, details, category),
            Post post => new PostReport(post, author, title, details, category),
            Comment comment => new CommentReport(comment, author, title, details, category),
            _ => throw new NotImplementedException("Reporting this entity is not possible")
        };
    }
}
