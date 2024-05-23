using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork uow) : BaseService<IReportService, ReportService>(uow), IReportService
{
    public IEnumerable<ReportType> GetAll<ReportType>()
        where ReportType : Report
    {
        AllowOnlyAdmins();

        return uow.Repository<ReportType>().GetAll();
    }

    public ReportType GetByGuid<ReportType>(Guid guid)
        where ReportType : Report
    {
        ReportType report = uow.Repository<ReportType>().GetOrThrow(guid);

        AllowOnlyUser(report.Author);

        return report;
    }

    public ReportType CreateReport<TargetType, ReportType>(Guid targetId, string title,
        string details, ReportCategory category)
        where TargetType : BaseEntity
        where ReportType : Report
    {
        AllowOnlyLoggedIn();

        var author = ActingUser!;

        var target = uow.Repository<TargetType>().GetOrThrow(targetId);

        var report = (ReportType)CreateSpecificReport(target, author, title, details, category);

        uow.Repository<ReportType>().Add(report);

        uow.Commit();

        return report;
    }

    public Report UpdateReport(Report newReport)
    {
        AllowOnlyAdmins();

        var report = uow.Repository<Report>().GetOrThrow(newReport.Guid);

        if (!report.IsOpen)
            throw new InvalidOperationException("Report is closed");

        report.Responder = ActingUser!;
        report.Feedback = newReport.Feedback;
        report.State = newReport.State;

        uow.Commit();

        return report;
    }

    public void DeleteReport(Guid guid)
    {
        var report = uow.Repository<Report>().GetOrThrow(guid);

        AllowOnlyUser(report.Author);

        uow.Repository<Report>().Delete(report);
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
