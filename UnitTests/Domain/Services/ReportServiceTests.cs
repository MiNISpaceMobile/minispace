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
    static readonly Guid g1 = Guid.Parse(new('1', 32));
    static readonly Guid g2 = Guid.Parse(new('2', 32));
    static readonly Guid g3 = Guid.Parse(new('3', 32));
    static readonly Guid g4 = Guid.Parse(new('4', 32));

    [TestInitialize]
    public void Setup()
    {
        var now = DateTime.Now;
        var st0 = new Student("user0", "user0@test.pl", "password");
        var st1 = new Student("user1", "user1@test.pl", "password") { IsOrganizer = true };
        var ev0 = new Event(st1, "test event", "test description", EventCategory.Uncategorized,
            now, now.AddDays(10), now.AddDays(11), "test location", 20, 20);
        var p0 = new Post(st1, ev0, "post");
        var c0 = new Comment(st0, p0, "first comment", null);
        var c1 = new Comment(st1, p0, "second comment", null);

        var re0 = new EventReport(ev0, st0, "event report", "report details", ReportCategory.Unknown)
        { Guid = g1 };
        var re1 = new PostReport(p0, st0, "post report", "report details", ReportCategory.Behaviour)
        { Guid = g2 };
        var re2 = new CommentReport(c0, st1, "comment report", "report details", ReportCategory.Behaviour)
        { Guid = g3 };
        var re3 = new CommentReport(c1, st0, "comment report", "report details", ReportCategory.Unknown)
        { Guid = g4 };

        unitOfWork = new DictionaryUnitOfWork([st0, st1, ev0, p0, c0, c1, re0, re1, re2, re3]);
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
        Action act = () => service.GetByGuid<Report>(Guid.Empty);

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
        Assert.AreEqual("Invalid guid", exception.Message);
    }

    [TestMethod]
    public void GetByGuid_GivenValidGuid_ReturnsReport()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        var result = service.GetByGuid<Report>(g1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(g1, result.Guid);
    }

    [TestMethod]
    public void GetByGuid_GivenValidGuidOfSpecificType_ReturnsReportOfSpecificType()
    {
        // Arrange
        ReportService service = new(unitOfWork);

        // Act
        Report result = service.GetByGuid<CommentReport>(g3);

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
        Action act = () => service.GetByGuid<CommentReport>(g1); // g1 is guid of EventReport

        // Assert
        var exception = Assert.ThrowsException<Exception>(act);
        Assert.AreEqual("Invalid guid", exception.Message);
    }
}
