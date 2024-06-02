using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.Storages;
using Infrastructure.UnitOfWorks;
using Microsoft.Extensions.Primitives;

namespace UnitTests.Domain.Services;

[TestClass]
public class PostServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private DictionaryUnitOfWork uow;
    private List<Event> events;
    private List<User> students;
    private Post post;

    private IPostService sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        User st0 = new User("tester0", "tester0@minispace.pw.edu.pl", "you_should_be_testing", now);
        students = new List<User> { st0 };

        Event ev0 = new Event(st0, "event0", "description0", EventCategory.Uncategorized, now, now, now, "here", null, null)
        { Guid = Guid.Parse("79b46c1c-96a6-4972-8f6f-ffd7edc33597") };
        Event ev1 = new Event(st0, "event1", "description1", EventCategory.Uncategorized, now, now, now, "here", null, null)
        { Guid = Guid.Parse("b091f07f-6ed7-4a80-bf7f-966765d3a13d") };
        events = new List<Event> { ev0, ev1 };

        post = new Post(st0, ev0, "test", "test");

        uow = new DictionaryUnitOfWork(Enumerable.Concat<BaseEntity>(students, events).Concat([post]));

        sut = new PostService(uow, new DictionaryStorage()).AsUser(st0.Guid);
    }

    #region CreatePost
    [TestMethod]
    public void CreatePost_EmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        Event @event = events.Last();
        User author = students.Last();
        string title = string.Empty;
        string content = string.Empty;

        // Act
        var action = () => sut.CreatePost(/*author.Guid, */@event.Guid, title, content);

        // Assert
        Assert.ThrowsException<EmptyContentException>(action);
    }

    [TestMethod]
    public void CreatePost_CorrectInput_AddPostToEvent()
    {
        // Arrange
        Event @event = events.Last();
        User author = students.Last();
        string title = "t";
        string content = "a";

        // Act
        Post post = sut.CreatePost(/*author.Guid, */@event.Guid, title, content);

        // Assert
        Assert.IsTrue(@event.Posts.Contains(post));
        Assert.IsTrue(uow.Repository<Post>().Get(post.Guid) is not null);
        Assert.IsTrue(AreEqual(post, author, @event, title, content));
    }
    #endregion

    #region GetPost
    [TestMethod]
    public void GetPost_NonexistentPost_ShouldThrowArgumentException()
    {
        // Arrange

        // Act
        var action = () => sut.GetPost(new Guid());

        // Assert
        Assert.ThrowsException<InvalidGuidException<Post>>(action);
    }

    [TestMethod]
    public void GetPost_CorrectInput_ShouldReturnCorrectPost()
    {
        // Arrange
        Event @event = events.Last();
        User author = students.Last();
        string title = "t";
        string content = "a";
        Post post = new Post(author, @event, title, content);
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

        // Act
        var action = () => sut.DeletePost(new Guid());

        // Assert
        Assert.ThrowsException<InvalidGuidException<Post>>(action);
    }

    [TestMethod]
    public void DeletePost_CorrectInput_ShouldDeleteCorrectPost()
    {
        // Arrange
        Event @event = events.Last();
        User author = students.Last();
        string title = "t";
        string content = "a";
        Post post = new Post(author, @event, title, content);
        uow.Repository<Post>().Add(post);
        @event.Posts.Add(post);

        // Act
        sut.DeletePost(post.Guid);

        // Assert
        Assert.IsFalse(@event.Posts.Contains(post));
        Assert.IsNull(uow.Repository<Post>().Get(post.Guid));
    }
    #endregion

    #region GetUsersPosts
    [TestMethod]
    public void GetUsersPosts_NonexistentUser_ShouldThrowInvalidGuidException()
    {
        // Arrange

        // Act
        var action = () => sut.AsUser(new Guid()).GetUsersPosts();

        // Assert
        Assert.ThrowsException<InvalidGuidException<User>>(action);
    }

    [TestMethod]
    public void GetUsersPosts_CorrectInput_ShouldReturnUsetsPosts()
    {
        // Arrange
        var user = students.Last();
        string title = "t";
        string content = "a";
        foreach (var e in events)
        {
            e.Posts.Add(new Post(user, e, title, content));
            user.SubscribedEvents.Add(e);
        }

        // Act
        var result = sut.AsUser(user.Guid).GetUsersPosts();

        // Assert
        Assert.AreEqual(events.Count, result.Count);
    }
    #endregion

    bool AreEqual(Post p, User author, Event @event, string title, string content)
    {
        return p.Author == author && p.Event == @event && p.Title == title && p.Content == content;
    }

    #region SetReaction
    [TestMethod]
    public void SetReaction_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(null).SetReaction(post.Guid, ReactionType.Like);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SetReaction_WasNullSetNull_DoesNothing()
    {
        sut.AsUser(students[0].Guid).SetReaction(post.Guid, null);

        Assert.IsFalse(post.Reactions.Any());
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SetReaction_WasLikeSetLike_DoesNothing()
    {
        sut.AsUser(students[0].Guid).SetReaction(post.Guid, ReactionType.Like);

        sut.AsUser(students[0].Guid).SetReaction(post.Guid, ReactionType.Like);

        var reaction = post.Reactions.Single();
        Assert.AreEqual(ReactionType.Like, reaction.Type);
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void SetReaction_WasLikeSetNull_SetsNull()
    {
        sut.AsUser(students[0].Guid).SetReaction(post.Guid, ReactionType.Like);

        sut.AsUser(students[0].Guid).SetReaction(post.Guid, null);

        Assert.IsFalse(post.Reactions.Any());
        Assert.AreEqual(2, uow.CommitCount);
    }

    [TestMethod]
    public void SetReaction_WasNullSetLike_SetsLike()
    {
        sut.AsUser(students[0].Guid).SetReaction(post.Guid, ReactionType.Like);

        var reaction = post.Reactions.Single();
        Assert.AreEqual(ReactionType.Like, reaction.Type);
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void SetReaction_WasLikeSetFunny_SetsFunny()
    {
        sut.AsUser(students[0].Guid).SetReaction(post.Guid, ReactionType.Like);

        sut.AsUser(students[0].Guid).SetReaction(post.Guid, ReactionType.Funny);

        var reaction = post.Reactions.Single();
        Assert.AreEqual(ReactionType.Funny, reaction.Type);
        Assert.AreEqual(2, uow.CommitCount);
    }
    #endregion SetReaction
}
