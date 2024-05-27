using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

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

    public IEnumerable<Report> GetReports(ICollection<ReportType> types, bool open, bool closed)
    {
        AllowOnlyUser(ActingUser);

        if (types.Count == 0 || (open == false && closed == false))
            return [];

        var reports = uow.Repository<Report>().GetAll();

        // Student gets only his reports, administrator gets all
        if (!ActingUser!.IsAdmin)
            reports = reports.Where(report => report.AuthorId == ActingUser.Guid);

        reports = reports.Where(report => types.Contains(report.ReportType));

        if (open ^ closed)
            reports = reports.Where(report => open ? report.IsOpen : !report.IsOpen);

        return reports;
    }

    public Report CreateReport(Guid targetId, string title, string details, ReportType type)
    {
        AllowOnlyLoggedIn();
        User user = ActingUser!;

        var report = type switch
        {
            ReportType.Event => CreateSpecificReport<Event>(targetId, user, title, details),
            ReportType.Post => CreateSpecificReport<Post>(targetId, user, title, details),
            ReportType.Comment => CreateSpecificReport<Comment>(targetId, user, title, details),
            _ => throw new InvalidOperationException("Wrong ReportType")
        };

        uow.Repository<Report>().Add(report);

        uow.Commit();

        return report;
    }

    public Report ReviewReport(Guid reportGuid, string? feedback)
    {
        AllowOnlyAdmins();

        var report = uow.Repository<Report>().GetOrThrow(reportGuid);

        if (!report.IsOpen)
            throw new InvalidOperationException("Report is closed");

        report.Responder = ActingUser;
        report.Feedback = feedback;
        report.IsOpen = false;
        report.UpdateDate = DateTime.Now;

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

    private Report CreateSpecificReport<TargetType>(Guid targetId, User author, string title, string details)
        where TargetType : BaseEntity
    {
        var target = uow.Repository<TargetType>().GetOrThrow(targetId);
        return target switch
        {
            Event @event => new EventReport(@event, author, title, details),
            Post post => new PostReport(post, author, title, details),
            Comment comment => new CommentReport(comment, author, title, details),
            _ => throw new InvalidOperationException("Reporting this entity is not possible")
        };
    }
}
