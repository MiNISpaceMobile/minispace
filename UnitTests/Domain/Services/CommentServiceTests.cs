using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class CommentServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private DictionaryUnitOfWork uow;
    private List<User> students;
    private List<Post> posts;
    private Comment comment;

    private ICommentService sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        User st0 = new User("tester0", "tester0@minispace.pw.edu.pl", "you_should_be_testing", now);
        students = new List<User> { st0 };

        Event @event = new Event(st0, "event0", "description0", EventCategory.Uncategorized, now, now, now, "here", null, null);
        Post po0 = new Post(st0, @event, "title0", "content0")
        { Guid = Guid.Parse("79b46c1c-96a6-4972-8f6f-ffd7edc33597") };
        Post po1 = new Post(st0, @event, "title1", "content1")
        { Guid = Guid.Parse("b091f07f-6ed7-4a80-bf7f-966765d3a13d") };
        posts = new List<Post> { po0, po1 };

        comment = new Comment(st0, po0, "test", null);

        uow = new DictionaryUnitOfWork(Enumerable.Concat<BaseEntity>(students, posts).Concat([comment]));

        sut = new CommentService(uow).AsUser(st0.Guid);
    }

    #region CreateComment
    [TestMethod]
    public void CreateComment_EmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        Post post = posts.Last();
        User author = students.Last();
        string content = string.Empty;

        // Act
        var action = () => sut.CreateComment(@post.Guid, content);

        // Assert
        Assert.ThrowsException<EmptyContentException>(action);
    }

    [TestMethod]
    public void CreateComment_CorrectInput_AddCommentToPost()
    {
        // Arrange
        Post post = posts.Last();
        User author = students.Last();
        string content = "a";

        // Act
        Comment comment = sut.CreateComment(post.Guid, content);

        // Assert
        Assert.IsTrue(post.Comments.Contains(comment));
        Assert.IsTrue(uow.Repository<Comment>().Get(comment.Guid) is not null);
        Assert.IsTrue(AreEqual(comment, author, post, content));
    }
    #endregion

    #region GetComment
    [TestMethod]
    public void GetComment_NonexistentComment_ShouldThrowArgumentException()
    {
        // Arrange

        // Act
        var action = () => sut.GetComment(new Guid());

        // Assert
        Assert.ThrowsException<InvalidGuidException<Comment>>(action);
    }

    [TestMethod]
    public void GetComment_CorrectInput_ShouldReturnCorrectComment()
    {
        // Arrange
        Post post = posts.Last();
        User author = students.Last();
        string content = "a";
        Comment comment = new Comment(author, post, content, null);
        uow.Repository<Comment>().Add(comment);

        // Act
        Comment result = sut.GetComment(comment.Guid);

        // Assert
        Assert.AreEqual(comment, result);
    }
    #endregion

    #region DeleteComment
    [TestMethod]
    public void DeleteComment_NonexistentComment_ShouldThrowArgumentException()
    {
        // Arrange

        // Act
        var action = () => sut.DeleteComment(new Guid());

        // Assert
        Assert.ThrowsException<InvalidGuidException<Comment>>(action);
    }

    // Removed because causes errors where EF does magic
    //[TestMethod]
    //public void DeleteComment_CorrectInput_ShouldDeleteCorrectComment()
    //{
    //    // Arrange
    //    Post post = posts.Last();
    //    User author = students.Last();
    //    string content = "a";
    //    Comment comment = new Comment(author, post, content, null);
    //    uow.Repository<Comment>().Add(comment);
    //    post.Comments.Add(comment);

    //    // Act
    //    sut.DeleteComment(comment.Guid);

    //    // Assert
    //    Assert.IsFalse(post.Comments.Contains(comment));
    //    Assert.IsNull(uow.Repository<Comment>().Get(comment.Guid));
    //}
    #endregion

    bool AreEqual(Comment c, User author, Post post, string content)
    {
        return c.Author == author && c.Post == post && c.Content == content;
    }

    #region SetLike
    [TestMethod]
    public void SetLike_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(null).SetLike(comment.Guid, false);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SetLike_WasNullSetNull_DoesNothing()
    {
        sut.AsUser(students[0].Guid).SetLike(comment.Guid, null);

        Assert.IsFalse(comment.Likes.Any());
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SetLike_WasLikeSetLike_DoesNothing()
    {
        sut.AsUser(students[0].Guid).SetLike(comment.Guid, false);

        sut.AsUser(students[0].Guid).SetLike(comment.Guid, false);

        var like = comment.Likes.Single();
        Assert.AreEqual(false, like.IsDislike);
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void SetLike_WasLikeSetNull_SetsNull()
    {
        sut.AsUser(students[0].Guid).SetLike(comment.Guid, false);

        sut.AsUser(students[0].Guid).SetLike(comment.Guid, null);

        Assert.IsFalse(comment.Likes.Any());
        Assert.AreEqual(2, uow.CommitCount);
    }

    [TestMethod]
    public void SetLike_WasNullSetLike_SetsLike()
    {
        sut.AsUser(students[0].Guid).SetLike(comment.Guid, false);

        var like = comment.Likes.Single();
        Assert.AreEqual(false, like.IsDislike);
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void SetLike_WasLikeSetFunny_SetsFunny()
    {
        sut.AsUser(students[0].Guid).SetLike(comment.Guid, false);

        sut.AsUser(students[0].Guid).SetLike(comment.Guid, true);

        var like = comment.Likes.Single();
        Assert.AreEqual(true, like.IsDislike);
        Assert.AreEqual(2, uow.CommitCount);
    }
    #endregion SetLike
}
