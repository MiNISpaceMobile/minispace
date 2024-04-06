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
#pragma warning restore CS8618 // Unassigned non-nullables
    static readonly Guid eventReportGuid = Guid.Parse(new('1', 32));
    static readonly Guid postReportGuid = Guid.Parse(new('2', 32));
    static readonly Guid commentReportGuid = Guid.Parse(new('3', 32));
    static readonly Guid studentGuid = Guid.Parse(new('4', 32));
    static readonly Guid eventGuid = Guid.Parse(new('5', 32));
    static readonly Guid adminGuid = Guid.Parse(new('6', 32));


    [TestInitialize]
    public void Setup()
    {
        var now = DateTime.Now;
        var st0 = new Student("user0", "user0@test.pl", "password") { Guid = studentGuid };
        var st1 = new Student("user1", "user1@test.pl", "password") { IsOrganizer = true };
        var ad0 = new Administrator("user2", "user2@test.pl", "password") { Guid = adminGuid };
        var ev0 = new Event(st1, "test event", "test description", EventCategory.Uncategorized,
            now, now.AddDays(10), now.AddDays(11), "test location", 20, 20)
        { Guid = eventGuid };
        var p0 = new Post(st1, ev0, "post");
        var c0 = new Comment(st0, p0, "first comment", null);
        var c1 = new Comment(st1, p0, "second comment", null);

        var re0 = new EventReport(ev0, st0, "event report", "report details", ReportCategory.Unknown)
        { Guid = eventReportGuid };
        var re1 = new PostReport(p0, st0, "post report", "report details", ReportCategory.Behaviour)
        { Guid = postReportGuid };
        var re2 = new CommentReport(c0, st1, "comment report", "report details", ReportCategory.Behaviour)
        {
            Guid = commentReportGuid,
            State = ReportState.Failure
        };
        var re3 = new CommentReport(c1, st0, "comment report", "report details", ReportCategory.Unknown);

        unitOfWork = new DictionaryUnitOfWork([st0, st1, ad0, ev0, p0, c0, c1, re0, re1, re2, re3]);
    }

    [TestMethod]
    public void GetAll_GivenReportType_ReturnsAllReports()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var result = service.GetAll<Report>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.Count());
    }

    [TestMethod]
    public void GetAll_GivenSpecificReportType_ReturnsReportsOnlyOfGivenType()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        IEnumerable<Report> result = service.GetAll<CommentReport>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(x => x is CommentReport));
    }

    [TestMethod]
    public void GetByGuid_GivenInvalidGuid_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.GetByGuid<Report>(Guid.Empty);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void GetByGuid_GivenValidGuid_ReturnsReport()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var result = service.GetByGuid<Report>(eventReportGuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(eventReportGuid, result.Guid);
    }

    [TestMethod]
    public void GetByGuid_GivenValidGuidOfSpecificType_ReturnsReportOfSpecificType()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        Report result = service.GetByGuid<CommentReport>(commentReportGuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is CommentReport);
    }

    [TestMethod]
    public void GetByGuid_GivenValidGuidOfSpecificType_ThrowsIfTypeIsDifferent()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.GetByGuid<CommentReport>(eventReportGuid);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidTargetGuid_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.CreateReport<Event, EventReport>(Guid.Empty, studentGuid, "title", "details", ReportCategory.Unknown);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidAuthorGuid_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.CreateReport<Event, EventReport>(eventGuid, Guid.Empty, "title", "details", ReportCategory.Unknown);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void CreateReport_InvalidRaportGuid_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var report = service.CreateReport<Event, EventReport>(eventGuid, studentGuid, "title", "details", ReportCategory.Unknown);

        // Assert
        Assert.IsTrue(unitOfWork.Repository<EventReport>().Get(report.Guid) is not null);
    }

    [TestMethod]
    public void UpdateReport_InvalidResponderGuid_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.UpdateReport(Guid.Empty, commentReportGuid, "feedback", ReportState.Success);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void UpdateReport_InvalidReportGuid_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.UpdateReport(adminGuid, Guid.Empty, "feedback", ReportState.Success);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void UpdateReport_ReportIsClosed_ThrowsError()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        void act() => service.UpdateReport(adminGuid, commentReportGuid, "feedback", ReportState.Success);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
    }

    [TestMethod]
    public void UpdateReport_ValidData_UpdatesReport()
    {
        // Arrange
        ReportService service = new(unitOfWork);
        var newFeedback = "feedback";
        var newState = ReportState.Success;

        // Act
        var updatedReport = service.UpdateReport(adminGuid, eventReportGuid, newFeedback, newState);

        // Assert
        Assert.AreEqual(newFeedback, updatedReport.Feedback);
        Assert.AreEqual(newState, updatedReport.State);
    }

}
