using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services;
using Domain.Services.Implementations;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class ReportServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private IUnitOfWork unitOfWork;
    private ReportService service;
    private EventReport evRe0;
    private PostReport pRe0;
    private CommentReport cRe0;
    private CommentReport cRe1;
    private User st0;
    private User st1;
    private User ad0;
    private Event ev0;
    private Post p0;
    private Comment c0;
    private Comment c1;
#pragma warning restore CS8618 // Unassigned non-nullables
    private static readonly ICollection<ReportType> AllTypes = [ReportType.Event, ReportType.Post, ReportType.Comment];
    [TestInitialize]
    public void Setup()
    {
        var now = DateTime.Now;
        st0 = new User("user0", "user0@test.pl", "password", now);
        st1 = new User("user1", "user1@test.pl", "password", now) { IsOrganizer = true };
        ad0 = new User("user2", "user2@test.pl", "password", now) { IsAdmin = true };
        ev0 = new Event(st1, "test event", "test description", EventCategory.Uncategorized,
            now, now.AddDays(10), now.AddDays(11), "test location", 20, 20);
        p0 = new Post(st1, ev0, "post");
        c0 = new Comment(st0, p0, "first comment", null);
        c1 = new Comment(st1, p0, "second comment", null);

        evRe0 = new EventReport(ev0, st0, "event report", "report details");
        pRe0 = new PostReport(p0, st0, "post report", "report details");
        cRe0 = new CommentReport(c0, st1, "comment report", "report details")
        { IsOpen =false};
        cRe1 = new CommentReport(c1, st0, "comment report", "report details");

        unitOfWork = new DictionaryUnitOfWork([st0, st1, ad0, ev0, p0, c0, c1, evRe0, pRe0, cRe0, cRe1]);
        service = new(unitOfWork);

    }

    #region GetAll
    [TestMethod]
    public void GetAll_ReportType_ReturnsAllReports()
    {
        var result = service.AsUser(ad0.Guid).GetAll<Report>();

        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.Count());
    }

    [TestMethod]
    public void GetAll_ConcreteReportType_ReturnsOnlyConcreteReports()
    {
        IEnumerable<Report> result = service.AsUser(ad0.Guid).GetAll<CommentReport>();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(x => x is CommentReport));
    }
    #endregion GetAll

    #region GetReports
    [TestMethod]
    public void GetReports_NotLoggedIn_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(null).GetReports([], true, true, true);

        var exception = Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void GetReports_UserTriesToGetAll_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(st0.Guid).GetReports([], true, true, true);

        var exception = Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void GetReports_User_GetsOnlyHisReports()
    {
        var reports = service.AsUser(st0.Guid).GetReports(AllTypes, true, true, false);

        Assert.IsNotNull(reports);
        Assert.IsTrue(reports.All(report => report.Author == st0));
    }

    [TestMethod]
    public void GetReports_Admin_GetsAllReports()
    {
        var reports = service.AsUser(ad0.Guid).GetReports(AllTypes, true, true, true);

        Assert.IsNotNull(reports);
        Assert.IsTrue(reports.Count() == 4);
    }

    [TestMethod]
    public void GetReports_SpecifiedType_GetsOnlyRaportsOfSpecifiedType()
    {
        var reports = service.AsUser(ad0.Guid).GetReports([ReportType.Event], true, true, true);

        Assert.IsNotNull(reports);
        Assert.IsTrue(reports.All(x => x is EventReport));
    }

    [TestMethod]
    public void GetReports_SpecifiedTypes_GetsOnlyRaportsOfSpecifiedTypes()
    {
        var reports = service.AsUser(ad0.Guid).GetReports([ReportType.Post, ReportType.Comment], true, true, true);

        Assert.IsNotNull(reports);
        Assert.IsTrue(reports.All(x => x is PostReport || x is CommentReport));
    }

    [TestMethod]
    public void GetReports_Open_GetsOnlyOpenReports()
    {
        var reports = service.AsUser(ad0.Guid).GetReports(AllTypes, true, false, true);

        Assert.IsNotNull(reports);
        Assert.IsTrue(reports.All(x => x.IsOpen));
    }

    [TestMethod]
    public void GetReports_Closed_GetsOnlyClosedReports()
    {
        var reports = service.AsUser(ad0.Guid).GetReports(AllTypes, false, true, true);

        Assert.IsNotNull(reports);
        Assert.IsTrue(reports.All(x => !x.IsOpen));
    }

    [TestMethod]
    public void GetReports_NoTypes_GetsEmptyIEnumerable()
    {
        var reports = service.AsUser(ad0.Guid).GetReports([], true, true, true);

        Assert.IsNotNull(reports);
        Assert.IsFalse(reports.Any());
    }

    [TestMethod]
    public void GetReports_NotOpenAndNotClosed_GetsEmptyIEnumerable()
    {
        var reports = service.AsUser(ad0.Guid).GetReports([], true, true, true);

        Assert.IsNotNull(reports);
        Assert.IsFalse(reports.Any());
    }
    #endregion GetReports

    #region GetByGuid
    [TestMethod]
    public void GetByGuid_InvalidGuid_ThrowsInvalidGuidException()
    {
        void act() => service.AsUser(ad0.Guid).GetByGuid<Report>(Guid.Empty);

        var exception = Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void GetByGuid_NonGeneric_InvalidGuid_ThrowsInvalidGuidException()
    {
        void act() => service.AsUser(ad0.Guid).GetByGuid(Guid.Empty);

        var exception = Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void GetByGuid_NotLoggedIn_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(null).GetByGuid<EventReport>(evRe0.Guid);

        var exception = Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void GetByGuid_NonGeneric_NotLoggedIn_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(null).GetByGuid(evRe0.Guid);

        var exception = Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void GetByGuid_WrongAuthorId_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(st1.Guid).GetByGuid<EventReport>(evRe0.Guid);

        var exception = Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void GetByGuid_NonGeneric_WrongAuthorId_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(st1.Guid).GetByGuid(evRe0.Guid);

        var exception = Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void GetByGuid_ValidGuid_ReturnsReport()
    {
        var result = service.AsUser(ad0.Guid).GetByGuid<Report>(evRe0.Guid);

        Assert.IsNotNull(result);
        Assert.AreEqual(evRe0.Guid, result.Guid);
    }

    [TestMethod]
    public void GetByGuid_NonGeneric_ValidGuid_ReturnsReport()
    {
        var result = service.AsUser(st0.Guid).GetByGuid(evRe0.Guid);

        Assert.IsNotNull(result);
        Assert.AreEqual(evRe0.Guid, result.Guid);
    }

    [TestMethod]
    public void GetByGuid_ValidGuidOfConcreteReportType_ReturnsReportOfConcreteType()
    {
        Report result = service.AsUser(ad0.Guid).GetByGuid<CommentReport>(cRe0.Guid);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is CommentReport);
    }

    [TestMethod]
    public void GetByGuid_ValidGuidOfConcreteReportType_ThrowsExceptionIfTypeIsDifferent()
    {
        void act() => service.AsUser(ad0.Guid).GetByGuid<CommentReport>(evRe0.Guid);

        var exception = Assert.ThrowsException<InvalidGuidException<CommentReport>>(act);
    }
    #endregion GetByGuid

    #region CreateReport
    [TestMethod]
    public void CreateReport_InvalidTargetEventGuid_ThrowsInvalidEventGuidException()
    {
        void act() => service.AsUser(st0.Guid).CreateReport(Guid.Empty, "title", "details", ReportType.Event);

        Assert.ThrowsException<InvalidGuidException<Event>>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidTargetPostGuid_ThrowsInvalidEventGuidException()
    {
        void act() => service.AsUser(st0.Guid).CreateReport(Guid.Empty, "title", "details", ReportType.Post);

        Assert.ThrowsException<InvalidGuidException<Post>>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidTargetCommentGuid_ThrowsInvalidEventGuidException()
    {
        void act() => service.AsUser(st0.Guid).CreateReport(Guid.Empty, "title", "details", ReportType.Comment);

        Assert.ThrowsException<InvalidGuidException<Comment>>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidTypeEnumValue_ThrowsInvalidOperationException()
    {
        void act() => service.AsUser(st0.Guid).CreateReport(Guid.Empty, "title", "details", (ReportType)3);

        Assert.ThrowsException<InvalidDomainEnumException>(act);
    }

    [TestMethod]
    public void CreateReport_NotLoggedIn_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(null).CreateReport(ev0.Guid, "title", "details", ReportType.Event);

        Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void CreateReport_ValidUserAndEventGuid_CreatesEventReport()
    {
        var report = service.AsUser(st0.Guid).CreateReport(ev0.Guid, "title", "details", ReportType.Event);

        Assert.IsNotNull(report);
        Assert.IsInstanceOfType<EventReport>(report);
        Assert.IsTrue(unitOfWork.Repository<Report>().Get(report.Guid) is not null);
    }

    [TestMethod]
    public void CreateReport_ValidUserAndPostGuid_CreatesPostReport()
    {
        var report = service.AsUser(st0.Guid).CreateReport(p0.Guid, "title", "details", ReportType.Post);

        Assert.IsNotNull(report);
        Assert.IsInstanceOfType<PostReport>(report);
        Assert.IsTrue(unitOfWork.Repository<Report>().Get(report.Guid) is not null);
    }

    [TestMethod]
    public void CreateReport_ValidUserAndCommentGuid_CreatesCommentReport()
    {
        var report = service.AsUser(st0.Guid).CreateReport(c0.Guid, "title", "details", ReportType.Comment);

        Assert.IsNotNull(report);
        Assert.IsInstanceOfType<CommentReport>(report);
        Assert.IsTrue(unitOfWork.Repository<Report>().Get(report.Guid) is not null);
    }
    #endregion CreateReport

    #region ReviewReport
    [TestMethod]
    public void ReviewReport_NotLoggedIn_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(null).ReviewReport(Guid.Empty, null);

        Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void ReviewReport_AsStudent_ThrowsUserUnauthorizedException()
    {
        void act() => service.AsUser(st0.Guid).ReviewReport(Guid.Empty, null);

        Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void ReviewReport_InvalidReportGuid_ThrowsInvalidGuidException()
    {
        void act() => service.AsUser(ad0.Guid).ReviewReport(Guid.Empty, null);

        Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void ReviewReport_ReportIsClosed_ThrowsInvalidOperationExceptionException()
    {
        void act() => service.AsUser(ad0.Guid).ReviewReport(cRe0.Guid, "feedback");

        Assert.ThrowsException<ClosedReportException>(act);
    }

    [TestMethod]
    public void ReviewReport_ReportIsOpen_UpdatesReport()
    {
        var oldResponder = cRe1.Responder;
        var oldFeedback = cRe1.Feedback;
        var oldState = cRe1.IsOpen;

        var report = service.AsUser(ad0.Guid).ReviewReport(cRe1.Guid, "feedback");

        Assert.IsNotNull(report);
        Assert.IsInstanceOfType<Report>(report);
        Assert.AreNotEqual(oldResponder, report.Responder);
        Assert.AreNotEqual(oldFeedback, report.Feedback);
        Assert.AreNotEqual(oldState, report.IsOpen);
    }
    #endregion ReviewReport

    #region DeleteReport
    [TestMethod]
    public void DeleteReport_NotLoggedIn_ThrowsUnauthorizedException()
    {
        void act() => service.AsUser(null).DeleteReport(cRe0.Guid);

        Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void DeleteReport_WrongUser_ThrowsUnauthorizedException()
    {
        void act() => service.AsUser(st0.Guid).DeleteReport(cRe0.Guid);

        Assert.ThrowsException<UserUnauthorizedException>(act);
    }

    [TestMethod]
    public void DeleteReport_InvalidGuid_ThrowsInvalidGuidException()
    {
        void act() => service.AsUser(ad0.Guid).DeleteReport(Guid.Empty);

         Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void DeleteReport_ValidUserAndReportGuid_DeletesReport()
    {
        service.AsUser(st0.Guid).DeleteReport(evRe0.Guid);

        Assert.IsFalse(unitOfWork.Repository<Report>().TryGet(evRe0.Guid, out _));
    }

    [TestMethod]
    public void DeleteReport_ValidAdminAndReportGuid_DeletesReport()
    {
        service.AsUser(ad0.Guid).DeleteReport(evRe0.Guid);

        Assert.IsFalse(unitOfWork.Repository<Report>().TryGet(evRe0.Guid, out _));
    }
    #endregion DeleteReport
}