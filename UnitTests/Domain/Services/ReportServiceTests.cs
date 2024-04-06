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


        var re0 = new EventReport(ev0, st0, "event report", "report details", ReportCategory.Unknown);
        var re1 = new PostReport(p0, st0, "post report", "report details", ReportCategory.Behaviour);
        var re2 = new CommentReport(c0, st1, "comment report", "report details", ReportCategory.Behaviour);
        var re3 = new CommentReport(c1, st0, "comment report", "report details", ReportCategory.Unknown);

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
    public void GetAll_GivenSpecificType_ReturnsAllReportsOnlyOfGivenType()
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
}
