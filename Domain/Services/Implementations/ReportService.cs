﻿using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork unitOfWork) : IReportService
{
    // TODO: Change all exception to custom exceptions with proper error information
    public IEnumerable<ReportType> GetAll<ReportType>()
        where ReportType : Report
    {
        return unitOfWork.Repository<ReportType>().GetAll();
    }

    public ReportType GetByGuid<ReportType>(Guid guid)
        where ReportType : Report
    {
        var report = unitOfWork.Repository<ReportType>().Get(guid);
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

    public Report UpdateReport(Report newReport)
    {
        var responderId = newReport.ResponderId ??
            throw new Exception("No responder guid");
        var responder = unitOfWork.Repository<Administrator>().Get(responderId) ??
            throw new Exception("Invalid responder guid");
        var report = unitOfWork.Repository<Report>().Get(newReport.Guid) ??
            throw new Exception("Invalid report guid");
        if (!report.IsOpen)
            throw new Exception("Report is closed");

        report.Responder = responder;
        report.Feedback = newReport.Feedback;
        report.State = newReport.State;

        unitOfWork.Commit();

        return report;
    }

    public void DeleteReport(Guid guid)
    {
        if (!unitOfWork.Repository<Report>().TryDelete(guid))
            throw new Exception("Invalid guid");

        unitOfWork.Commit();
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