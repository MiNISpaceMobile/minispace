using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class EventServicetTests
{
    private IUnitOfWork uow;
    private List<Event> events;
    private List<Student> students;

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        Student st0 = new Student("tester0", "tester0@minispace.pw.edu.pl", "you_should_be_testing");
        students = new List<Student> { st0 };

        Event ev0 = new Event(st0, "event0", "description0", EventCategory.Uncategorized, now, now, now, "here", null, null)
        { Guid = Guid.Parse("79b46c1c-96a6-4972-8f6f-ffd7edc33597") };
        Event ev1 = new Event(st0, "event1", "description1", EventCategory.Uncategorized, now, now, now, "here", null, null)
        { Guid = Guid.Parse("b091f07f-6ed7-4a80-bf7f-966765d3a13d") };
        events = new List<Event> { ev0, ev1 };

        uow = new DictionaryUnitOfWork(Enumerable.Concat<BaseEntity>(students, events));
    }

    [TestMethod]
    public void GetEvent_Nonexistent_Null()
    {
        // Arrange
        EventService service = new EventService(uow);
        Guid guid = Guid.Parse("a0403bb2-3db8-4142-9270-959f962c01be");

        // Act
        Event? result = service.GetEvent(guid);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetEvent_Last_NotNull()
    {
        // Arrange
        EventService service = new EventService(uow);
        Guid guid = events.Last().Guid;

        // Act
        Event? result = service.GetEvent(guid);

        // Assert
        Assert.IsNotNull(result);
    }

    //[TestMethod]
    //public void ChangeTest()
    //{
    //    Event e = uow.Repository<Event>().Get(events.First().Guid)!;
        
    //    e.Capacity = 30;

    //    Assert.AreEqual(30, events.First().Capacity);
    //}

    //[TestMethod]
    //public void AddTest()
    //{
    //    Event e = new Event(students.First(), "aaa", "bbb", EventCategory.Uncategorized, DateTime.Now, DateTime.Now, DateTime.Now, "ccc", 1, null)
    //    { Guid = Guid.NewGuid() };

    //    uow.Repository<Event>().Add(e);

    //    Assert.IsNotNull(uow.Repository<Event>().Get(e.Guid));
    //}
}
