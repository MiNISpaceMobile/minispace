using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;
using System.Net;

namespace UnitTests.Domain.Services;

[TestClass]
public class PostServiceTests
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

    #region CreatePost
    [TestMethod]
    public void CreatePost_EmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        PostService sut = new PostService(uow);
        Event @event = events.Last();
        Student author = students.Last();
        string content = string.Empty;

        // Act
        var action = () => sut.CreatePost(author.Guid, @event.Guid, content);

        // Assert
        Assert.ThrowsException<EmptyContentException>(action);
    }

    [TestMethod]
    public void CreatePost_CorrectInput_AddPostToEvent()
    {
        // Arrange
        PostService sut = new PostService(uow);
        Event @event = events.Last();
        Student author = students.Last();
        string content = "a";

        // Act
        Post post = sut.CreatePost(author.Guid, @event.Guid, content);

        // Assert
        Assert.IsTrue(@event.Posts.Contains(post));
        Assert.IsTrue(uow.Repository<Post>().Get(post.Guid) is not null);
        Assert.IsTrue(AreEqual(post, author, @event, content));
    }
    #endregion

    #region GetPost
    [TestMethod]
    public void GetPost_NonexistentPost_ShouldThrowArgumentException()
    {
        // Arrange
        PostService sut = new PostService(uow);

        // Act
        var action = () => sut.GetPost(new Guid());

        // Assert
        Assert.ThrowsException<InvalidGuidException<Post>>(action);
    }

    [TestMethod]
    public void GetPost_CorrectInput_ShouldReturnCorrectPost()
    {
        // Arrange
        PostService sut = new PostService(uow);
        Event @event = events.Last();
        Student author = students.Last();
        string content = "a";
        Post post = new Post(author, @event, content);
        uow.Repository<Post>().Add(post);

        // Act
        Post result = sut.GetPost(post.Guid);

        // Assert
        Assert.AreEqual(post, result);
    }
    #endregion

    #region DeletePost
    [TestMethod]
    public void DeletePost_NonexistentPost_ShouldThrowArgumentException()
    {
        // Arrange
        PostService sut = new PostService(uow);

        // Act
        var action = () => sut.DeletePost(new Guid());

        // Assert
        Assert.ThrowsException<InvalidGuidException<Post>>(action);
    }

    [TestMethod]
    public void DeletePost_CorrectInput_ShouldDeleteCorrectPost()
    {
        // Arrange
        PostService sut = new PostService(uow);
        Event @event = events.Last();
        Student author = students.Last();
        string content = "a";
        Post post = new Post(author, @event, content);
        uow.Repository<Post>().Add(post);
        @event.Posts.Add(post);

        // Act
        sut.DeletePost(post.Guid);

        // Assert
        Assert.IsFalse(@event.Posts.Contains(post));
        Assert.IsNull(uow.Repository<Post>().Get(post.Guid));
    }
    #endregion

    bool AreEqual(Post p, Student author, Event @event, string content)
    {
        return p.Author == author && p.Event == @event && p.Content == content;
    }
}
