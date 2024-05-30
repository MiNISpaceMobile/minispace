using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.Storages;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class EventServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private IUnitOfWork uow;
    private List<Event> events;
    private List<User> students;

    private IEventService sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        User st0 = new User("tester0", "tester0@minispace.pw.edu.pl", "you_should_be_testing", now) { IsOrganizer = true };
        students = new List<User> { st0 };

        Event ev0 = new Event(st0, "event0", "description0", EventCategory.Uncategorized, now, now, now, "here", null, null)
        { Guid = Guid.Parse("79b46c1c-96a6-4972-8f6f-ffd7edc33597") };
        Event ev1 = new Event(st0, "event1", "description1", EventCategory.Uncategorized, now, now, now, "here", null, null)
        { Guid = Guid.Parse("b091f07f-6ed7-4a80-bf7f-966765d3a13d") };
        events = new List<Event> { ev0, ev1 };

        uow = new DictionaryUnitOfWork(Enumerable.Concat<BaseEntity>(students, events));
        DictionaryStorage storage = new DictionaryStorage();

        sut = new EventService(uow, new PostService(uow, storage), storage).AsUser(st0.Guid);
    }

    #region GetEvent
    [TestMethod]
    public void GetEvent_Nonexistent_ShouldThrowArgumentException()
    {
        // Arrange
        Guid guid = Guid.Parse("a0403bb2-3db8-4142-9270-959f962c01be");

        // Act
        Action action = () => sut.GetEvent(guid);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void GetEvent_Last_NotNull()
    {
        // Arrange
        Guid guid = events.Last().Guid;

        // Act
        Event result = sut.GetEvent(guid);

        // Assert
        Assert.IsNotNull(result);
    }
    #endregion

    #region CreateEvent
    [TestMethod]
    public void CreateEvent_NotLoggedIn_ShouldThrowUserUnathorized()
    {
        // Arrange
        DateTime now = DateTime.Now;

        // Act
        var action = () => sut.AsUser(null).CreateEvent("event2", "description2", EventCategory.Uncategorized, now, now, now, "here", null, null);

        // Assert
        Assert.ThrowsException<UserUnauthorizedException>(action);
    }

    [TestMethod]
    public void CreateEvent_CorrectEvent_ShouldAddEventToDB()
    {
        // Arrange
        DateTime now = DateTime.Now;

        // Act
        Event newEvent = sut.CreateEvent("event2", "description2", EventCategory.Uncategorized, now, now, now, "here", null, null);

        // Assert 
        Assert.AreEqual(newEvent, uow.Repository<Event>().Get(newEvent.Guid));
    }
    #endregion

    #region DeleteEvent
    [TestMethod]
    public void DeleteEvent_NonexistentEvent_ShouldThrowInvalidGuidException()
    {
        // Arrange

        // Act
        Action action = () => sut.DeleteEvent(Guid.Empty);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void DeleteEvent_CorrectEvent_ShouldDeleteEvent()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();
        student.SubscribedEvents.Add(@event);
        @event.Interested.Add(student);
        // Event posts
        uow.Repository<Event>().Add(@event);
        Post post1 = new Post(student, @event, "t", "a");
        uow.Repository<Post>().Add(post1);
        Post post2 = new Post(student, @event, "t", "a");
        uow.Repository<Post>().Add(post2);
        @event.Posts.Add(post1);
        @event.Posts.Add(post2);

        // Act
        sut.DeleteEvent(@event.Guid);

        // Assert
        Assert.IsNull(uow.Repository<Event>().Get(@event.Guid));
        Assert.IsNull(uow.Repository<Post>().Get(post1.Guid));
        Assert.IsNull(uow.Repository<Post>().Get(post2.Guid));
        Assert.IsTrue(@event.Interested.Count == 0);
        Assert.IsTrue(@event.Posts.Count == 0);
        Assert.IsFalse(student.SubscribedEvents.Contains(@event));
    }
    #endregion

    #region UpdateEvent
    [TestMethod]
    public void UpdateEvent_NonexistentEvent_ShouldThrowArgumentException()
    {
        // Arrange
        DateTime now = DateTime.Now;
        Event newEvent = new Event(students.Last(), "event2", "description2", EventCategory.Uncategorized, now, now, now, "here", null, null);

        // Act
        var action = () => sut.UpdateEvent(newEvent);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void UpdateEvent_CorrectEvent_ShouldUpdateEvent()
    {
        // Arrange
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

        // Act
        Action action = () => sut.TryAddParticipant(Guid.Empty);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void TryAddParticipant_FullEvent_ShouldReturnFalse()
    {
        // Arrange
        Event @event = events.Last();
        @event.Capacity = 0;

        // Act
        var result = sut.TryAddParticipant(@event.Guid);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryAddParticipant_AlreadyParticipating_ShouldReturnFalse()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();
        @event.Participants.Add(student);

        // Act
        var result = sut.TryAddParticipant(@event.Guid);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(@event.Participants.Contains(student));
    }

    [TestMethod]
    public void TryParticipate_NoGivenCapacity_ShouldAddParticipant()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();

        // Act
        var result = sut.TryAddParticipant(@event.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(@event.Participants.Contains(student));
    }

    [TestMethod]
    public void TryAddParticipant_AvailablePlace_ShouldAddParticipant()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();
        @event.Capacity = 100;

        // Act
        var result = sut.TryAddParticipant(@event.Guid);

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

        // Act
        Action action = () => sut.TryAddInterested(Guid.Empty);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void TryAddParticipant_AlreadyInterested_ShouldReturnFalse()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();
        @event.Interested.Add(student);

        // Act
        var result = sut.TryAddInterested(@event.Guid);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(@event.Interested.Contains(student));
    }

    [TestMethod]
    public void TryAddInterested_CorrectRequest_ShouldAddInterested()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();

        // Act
        var result = sut.TryAddInterested(@event.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(@event.Interested.Contains(student));
        Assert.IsTrue(student.SubscribedEvents.Contains(@event));
    }
    #endregion

    #region TryRemoveParticipant
    [TestMethod]
    public void TryRemoveParticipant_NonexistentEvent_ShouldThrowInvalidGuidException()
    {
        // Arrange

        // Act
        Action action = () => sut.TryRemoveParticipant(Guid.Empty);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void TryRemoveParticipant_NotParticipating_ShouldReturnFalse()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();

        // Act
        var result = sut.TryRemoveParticipant(@event.Guid);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryRemoveParticipant_Participating_ShouldRemoveParticipant()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();
        @event.Participants.Add(student);
        student.SubscribedEvents.Add(@event);

        // Act
        var result = sut.TryRemoveParticipant(@event.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(@event.Participants.Contains(student));
        Assert.IsFalse(student.SubscribedEvents.Contains(@event));
    }
    #endregion

    #region TryRemoveInterested
    [TestMethod]
    public void TryRemoveInterested_NonexistentEvent_ShouldThrowInvalidGuidException()
    {
        // Arrange

        // Act
        Action action = () => sut.TryRemoveInterested(Guid.Empty);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void TryRemoveInterested_NotInterested_ShouldReturnFalse()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();

        // Act
        var result = sut.TryRemoveInterested(@event.Guid);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryRemoveParticipant_Interested_ShouldRemoveInterested()
    {
        // Arrange
        Event @event = events.Last();
        User student = students.Last();
        @event.Interested.Add(student);
        student.SubscribedEvents.Add(@event);

        // Act
        var result = sut.TryRemoveInterested(@event.Guid);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(@event.Interested.Contains(student));
        Assert.IsFalse(student.SubscribedEvents.Contains(@event));
    }
    #endregion

    #region AddFeedback
    [TestMethod]
    public void AddFeedback_NonexistentEvent_ShouldThrowInvalidGuidException()
    {
        // Arrange

        // Act
        Action action = () => sut.AddFeedback(Guid.Empty, string.Empty);

        // Assert
        Assert.ThrowsException<InvalidGuidException<Event>>(action);
    }

    [TestMethod]
    public void AddFeedback_EmptyContent_ShouldThrowEmptyContentException()
    {
        // Arrange

        // Act
        Action action = () => sut.AddFeedback(events.Last().Guid, string.Empty);

        // Assert
        Assert.ThrowsException<EmptyContentException>(action);
    }

    [TestMethod]
    public void AddFeedback_AlreadyGivenFeedback_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Event @event = events.Last();
        User author = students.First();
        Feedback feedback = new Feedback(author, @event, "a");
        @event.Feedback.Add(feedback);

        // Act
        Action action = () => sut.AddFeedback(@event.Guid, "b");

        // Assert
        Assert.ThrowsException<InvalidOperationException>(action);
    }

    [TestMethod]
    public void AddFeedback_CorrectInput_ShouldAddFeedback()
    {
        // Arrange
        Event @event = events.Last();
        User author = students.First();

        // Act
        Feedback result = sut.AddFeedback(@event.Guid, "a");

        // Assert
        Assert.IsTrue(@event.Feedback.Count == 1);
    }
    #endregion
}
