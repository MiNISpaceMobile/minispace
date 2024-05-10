using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class ReportServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private IUnitOfWork unitOfWork;
    private EventReport evRe0;
    private PostReport pRe0;
    private CommentReport cRe0;
    private CommentReport cRe1;
    private Student st0;
    private Student st1;
    private Administrator ad0;
    private Event ev0;
    private Post p0;
    private Comment c0;
    private Comment c1;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void Setup()
    {
        var now = DateTime.Now;
        st0 = new Student("user0", "user0@test.pl", "password");
        st1 = new Student("user1", "user1@test.pl", "password") { IsOrganizer = true };
        ad0 = new Administrator("user2", "user2@test.pl", "password");
        ev0 = new Event(st1, "test event", "test description", EventCategory.Uncategorized,
            now, now.AddDays(10), now.AddDays(11), "test location", 20, 20);
        p0 = new Post(st1, ev0, "post");
        c0 = new Comment(st0, p0, "first comment", null);
        c1 = new Comment(st1, p0, "second comment", null);

        evRe0 = new EventReport(ev0, st0, "event report", "report details", ReportCategory.Unknown);
        pRe0 = new PostReport(p0, st0, "post report", "report details", ReportCategory.Behaviour);
        cRe0 = new CommentReport(c0, st1, "comment report", "report details", ReportCategory.Behaviour)
        { State = ReportState.Failure };
        cRe1 = new CommentReport(c1, st0, "comment report", "report details", ReportCategory.Unknown);

        unitOfWork = new DictionaryUnitOfWork([st0, st1, ad0, ev0, p0, c0, c1, evRe0, pRe0, cRe0, cRe1]);
    }

    #region GetAll
    [TestMethod]
    public void GetAll_ReportType_ReturnsAllReports()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var result = service.AsUser(ad0.Guid).GetAll<Report>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.Count());
    }

    [TestMethod]
    public void GetAll_ConcreteReportType_ReturnsOnlyConcreteReports()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        IEnumerable<Report> result = service.AsUser(ad0.Guid).GetAll<CommentReport>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(x => x is CommentReport));
    }
    #endregion GetAll

    #region GetByGuid
    [TestMethod]
    public void GetByGuid_InvalidGuid_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.AsUser(ad0.Guid).GetByGuid<Report>(Guid.Empty);

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void GetByGuid_ValidGuid_ReturnsReport()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var result = service.AsUser(ad0.Guid).GetByGuid<Report>(evRe0.Guid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(evRe0.Guid, result.Guid);
    }

    [TestMethod]
    public void GetByGuid_ValidGuidOfConcreteReportType_ReturnsReportOfConcreteType()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        Report result = service.AsUser(ad0.Guid).GetByGuid<CommentReport>(cRe0.Guid);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is CommentReport);
    }

    [TestMethod]
    public void GetByGuid_ValidGuidOfConcreteReportType_ThrowsExceptionIfTypeIsDifferent()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.AsUser(ad0.Guid).GetByGuid<CommentReport>(evRe0.Guid);

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<CommentReport>>(act);
    }
    #endregion GetByGuid

    #region CreateReport
    [TestMethod]
    public void CreateReport_InvalidTargetGuid_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.AsUser(st0.Guid).CreateReport<Event, EventReport>(Guid.Empty, st0.Guid, "title", "details", ReportCategory.Unknown);

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<Event>>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidAuthorGuid_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.AsUser(st0.Guid).CreateReport<Event, EventReport>(ev0.Guid, Guid.Empty, "title", "details", ReportCategory.Unknown);

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<User>>(act);
    }

    [TestMethod]
    public void CreateReport_ValidGuids_CreatesReport()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var report = service.AsUser(st0.Guid).CreateReport<Event, EventReport>(ev0.Guid, st0.Guid, "title", "details", ReportCategory.Unknown);

        // Assert
        Assert.IsNotNull(report);
        Assert.IsTrue(unitOfWork.Repository<EventReport>().Get(report.Guid) is not null);
    }
    #endregion CreateReport

    #region UpdateReport
    [TestMethod]
    public void UpdateReport_InvalidResponderGuid_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        var newStudent = new Student("abc", "abc", "abc");
        var newReport = new CommentReport(c0, newStudent, "title", "details", ReportCategory.Bug) { Guid = cRe1.Guid };

        // Act
        void act() => service.AsUser(ad0.Guid).UpdateReport(newReport, Guid.NewGuid());

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<Administrator>>(act);
    }

    [TestMethod]
    public void UpdateReport_InvalidReportGuid_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);
        var newReport = new CommentReport(c0, st0, "title", "details", ReportCategory.Bug);

        // Act
        void act() => service.AsUser(ad0.Guid).UpdateReport(newReport, ad0.Guid);

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void UpdateReport_ReportIsClosed_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);
        var newReport = new CommentReport(c0, st1, "title", "details", ReportCategory.Bug) { Guid = cRe0.Guid };

        // Act
        void act() => service.AsUser(ad0.Guid).UpdateReport(newReport, ad0.Guid);

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(act);
    }
    #endregion UpdateReport

    #region DeleteReport
    [TestMethod]
    public void DeleteReport_InvalidGuid_ThrowsException()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.AsUser(ad0.Guid).DeleteReport(Guid.Empty);

        // Assert
        var exception = Assert.ThrowsException<InvalidGuidException<Report>>(act);
    }

    [TestMethod]
    public void DeleteReport_ValidGuid_DeletesReport()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        service.AsUser(st0.Guid).DeleteReport(evRe0.Guid);

        // Assert
        Assert.IsFalse(unitOfWork.Repository<Report>().TryGet(evRe0.Guid, out _));
    }
    #endregion DeleteReport
}