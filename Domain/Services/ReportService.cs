
using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork unitOfWork)
{
    public IEnumerable<ReportType> GetAll<ReportType>() 
        where ReportType : Report
    {
        return unitOfWork.Repository<ReportType>().GetAll();
    }

    public ReportType GetByGuid<ReportType>(Guid guid) 
        where ReportType : Report
    {
        var report = unitOfWork.Repository<ReportType>().Get(guid);
        // TODO: Throw custom exception
        return report is not null ? report : throw new Exception("Invalid report guid");
    }

    public ReportType CreateReport<TargetType, ReportType>(Guid targetId, Guid authorId, string title,
        string details, ReportCategory category) 
        where TargetType : BaseEntity
        where ReportType : Report
    {
        var target = unitOfWork.Repository<TargetType>().Get(targetId) ??
            throw new Exception("Invalid target guid");
        var author = unitOfWork.Repository<Student>().Get(authorId) ??
            throw new Exception("Invalid author guid");

        var report = (ReportType)CreateSpecificReport(target, author, title, details, category);

        unitOfWork.Repository<ReportType>().Add(report);
        unitOfWork.Commit();

        return report;
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
            _ => throw new NotImplementedException()
        };
    }
}
