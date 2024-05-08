using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork uow) : BaseService<IReportService, ReportService>(uow), IReportService
{
    public IEnumerable<ReportType> GetAll<ReportType>()
        where ReportType : Report
    {
        return uow.Repository<ReportType>().GetAll();
    }

    public ReportType GetByGuid<ReportType>(Guid guid)
        where ReportType : Report
        => uow.Repository<ReportType>().GetOrThrow(guid);

    public ReportType CreateReport<TargetType, ReportType>(Guid targetId, Guid authorId, string title,
        string details, ReportCategory category)
        where TargetType : BaseEntity
        where ReportType : Report
    {
        var target = uow.Repository<TargetType>().GetOrThrow(targetId);
        var author = uow.Repository<Student>().GetOrThrow(authorId);

        var report = (ReportType)CreateSpecificReport(target, author, title, details, category);

        uow.Repository<ReportType>().Add(report);

        uow.Commit();

        return report;
    }

    public Report UpdateReport(Report newReport, Guid responderId)
    {
        var report = uow.Repository<Report>().GetOrThrow(newReport.Guid);
        var responder = uow.Repository<Administrator>().GetOrThrow(responderId);
        if (!report.IsOpen)
            throw new InvalidOperationException("Report is closed");

        report.Responder = responder;
        report.Feedback = newReport.Feedback;
        report.State = newReport.State;

        uow.Commit();

        return report;
    }

    public void DeleteReport(Guid guid)
    {
        if (!uow.Repository<Report>().TryDelete(guid))
            throw new InvalidGuidException<Report>();

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
            _ => throw new InvalidOperationException("Reporting this entity is not possible")
        };
    }
}
