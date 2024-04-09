using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class EventServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private IUnitOfWork uow;
    private List<Event> events;
    private List<Student> students;
#pragma warning restore CS8618 // Unassigned non-nullables

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

    #region GetEvent
    [TestMethod]
    public void GetEvent_Nonexistent_ShouldThrowArgumentException()
    {
        // Arrange
        EventService service = new EventService(uow);
        Guid guid = Guid.Parse("a0403bb2-3db8-4142-9270-959f962c01be");

        // Act
        Action action = () => service.GetEvent(guid);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void GetEvent_Last_NotNull()
    {
        // Arrange
        EventService service = new EventService(uow);
        Guid guid = events.Last().Guid;

        // Act
        Event result = service.GetEvent(guid);

        // Assert
        Assert.IsNotNull(result);
    }
    #endregion

    #region CreateEvent
    [TestMethod]
    public void CreateEvent_NonexistentStudent_ShouldThrowArgumentException()
    {
        // Arrange
        EventService sut = new EventService(uow);
        DateTime now = DateTime.Now;

        // Act
        var action = () => sut.CreateEvent(new Guid(), "event2", "description2", EventCategory.Uncategorized, now, now, now, "here", null, null);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("Nonexistent student", ex.Message);
    }

    [TestMethod]
    public void CreateEvent_CorrectEvent_ShouldAddEventToDB()
    {
        // Arrange
        EventService sut = new EventService(uow);
        DateTime now = DateTime.Now;

        // Act
        Event newEvent = sut.CreateEvent(students.Last().Guid, "event2", "description2", EventCategory.Uncategorized, now, now, now, "here", null, null);

        // Assert 
        Assert.AreEqual(newEvent, uow.Repository<Event>().Get(newEvent.Guid));
    }
    #endregion

    #region UpdateEvent
    [TestMethod]
    public void UpdateEvent_NonexistentEvent_ShouldThrowArgumentException()
    {
        // Arrange
        EventService sut = new EventService(uow);
        DateTime now = DateTime.Now;
        Event newEvent = new Event(students.Last(), "event2", "description2", EventCategory.Uncategorized, now, now, now, "here", null, null);

        // Act
        var action = () => sut.UpdateEvent(newEvent);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("Nonexistent event", ex.Message);
    }

    [TestMethod]
    public void UpdateEvent_CorrectEvent_ShouldUpdateEvent()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event toUpdate = events.Last();
        string newTitle = "a";

        // Act
        toUpdate.Title = newTitle;
        sut.UpdateEvent(toUpdate);

        // Assert
        Assert.AreEqual(toUpdate, uow.Repository<Event>().Get(toUpdate.Guid));
    }
    #endregion

    #region TryAddParticipant
    [TestMethod]
    public void TryAddParticipant_NonexistentEvent_ShouldThrowArgumentException()
    {
        // Arrange
        EventService sut = new EventService(uow);

        // Act
        Action action = () => sut.TryAddParticipant(Guid.Empty, students.Last().Guid);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("Nonexistent object", ex.Message);
    }

    [TestMethod]
    public void TryAddParticipant_FullEvent_ShouldReturnFalse()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event @event = events.Last();
        @event.Capacity = 0;

        // Act
        var result = sut.TryAddParticipant(@event.Guid, students.Last().Guid);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryAddParticipant_AlreadyParticipating_ShouldReturnFalse()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event @event = events.Last();
        Student student = students.Last();
        @event.Participants.Add(student);

        // Act
        var result = sut.TryAddParticipant(@event.Guid, student.Guid);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(@event.Participants.Contains(student));
    }

    [TestMethod]
    public void TryParticipate_NoGivenCapacity_ShouldAddParticipant()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event @event = events.Last();
        Student student = students.Last();

        // Act
        var result = sut.TryAddParticipant(@event.Guid, student.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(@event.Participants.Contains(student));
    }

    [TestMethod]
    public void TryAddParticipant_AvailablePlace_ShouldAddParticipant()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event @event = events.Last();
        Student student = students.Last();
        @event.Capacity = 100;

        // Act
        var result = sut.TryAddParticipant(@event.Guid, student.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(@event.Participants.Contains(student));
    }
    #endregion

    #region TryAddInterested
    [TestMethod]
    public void TryAddInterested_NonexistentEvent_ShouldThrowArgumentException()
    {
        // Arrange
        EventService sut = new EventService(uow);

        // Act
        Action action = () => sut.TryAddInterested(Guid.Empty, students.Last().Guid);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("Nonexistent object", ex.Message);
    }

    [TestMethod]
    public void TryAddParticipant_AlreadyInterested_ShouldReturnFalse()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event @event = events.Last();
        Student student = students.Last();
        @event.Interested.Add(student);

        // Act
        var result = sut.TryAddInterested(@event.Guid, student.Guid);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(@event.Interested.Contains(student));
    }

    [TestMethod]
    public void TryAddInterested_CorrectRequest_ShouldAddInterested()
    {
        // Arrange
        EventService sut = new EventService(uow);
        Event @event = events.Last();
        Student student = students.Last();

        // Act
        var result = sut.TryAddInterested(@event.Guid, student.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(@event.Interested.Contains(student));
        Assert.IsTrue(student.SubscribedEvents.Contains(@event));
    }
    #endregion
}
