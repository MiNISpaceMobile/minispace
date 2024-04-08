using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;
using System.Net;

namespace UnitTests.Domain.Services;

[TestClass]
public class CommentServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private IUnitOfWork uow;
    private List<Student> students;
    private List<Post> posts;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        Student st0 = new Student("tester0", "tester0@minispace.pw.edu.pl", "you_should_be_testing");
        students = new List<Student> { st0 };

        Event @event = new Event(st0, "event0", "description0", EventCategory.Uncategorized, now, now, now, "here", null, null);
        Post po0 = new Post(st0, @event, "content0")
        { Guid = Guid.Parse("79b46c1c-96a6-4972-8f6f-ffd7edc33597") };
        Post po1 = new Post(st0, @event, "content1")
        { Guid = Guid.Parse("b091f07f-6ed7-4a80-bf7f-966765d3a13d") };
        posts = new List<Post> { po0, po1 };

        uow = new DictionaryUnitOfWork(Enumerable.Concat<BaseEntity>(students, posts));
    }

    [TestMethod]
    public void CreateComment_EmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        CommentService sut = new CommentService(uow);
        Post post = posts.Last();
        Student author = students.Last();
        string content = string.Empty;

        // Act
        var action = () => sut.CreateComment(author.Guid, @post.Guid, content);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(ex.Message, "Arguments must not be empty");
    }

    [TestMethod]
    public void CreateComment_CorrectInput_AddCommentToPost()
    {
        // Arrange
        CommentService sut = new CommentService(uow);
        Post post = posts.Last();
        Student author = students.Last();
        string content = "a";

        // Act
        Comment comment = sut.CreateComment(author.Guid, post.Guid, content);

        // Assert
        Assert.IsTrue(post.Comments.Contains(comment));
        Assert.IsTrue(uow.Repository<Comment>().Get(comment.Guid) is not null);
        Assert.IsTrue(AreEqual(comment, author, post, content));
    }

    [TestMethod]
    public void GetComment_NonexistentComment_ShouldThrowArgumentException()
    {
        // Arrange
        CommentService sut = new CommentService(uow);

        // Act
        var action = () => sut.GetComment(new Guid());

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(ex.Message, "Nonexistent comment");
    }

    [TestMethod]
    public void GetComment_CorrectInput_ShouldReturnCorrectComment()
    {
        // Arrange
        CommentService sut = new CommentService(uow);
        Post post = posts.Last();
        Student author = students.Last();
        string content = "a";
        Comment comment = new Comment(author, post, content, null);
        uow.Repository<Comment>().Add(comment);

        // Act
        Comment result = sut.GetComment(comment.Guid);

        // Assert
        Assert.AreEqual(comment, result);
    }

    [TestMethod]
    public void DeleteComment_NonexistentComment_ShouldThrowArgumentException()
    {
        // Arrange
        CommentService sut = new CommentService(uow);

        // Act
        var action = () => sut.DeleteComment(new Guid());

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(ex.Message, "Nonexistent comment");
    }

    [TestMethod]
    public void DeleteComment_CorrectInput_ShouldDeleteCorrectComment()
    {
        // Arrange
        CommentService sut = new CommentService(uow);
        Post post = posts.Last();
        Student author = students.Last();
        string content = "a";
        Comment comment = new Comment(author, post, content, null);
        uow.Repository<Comment>().Add(comment);
        post.Comments.Add(comment);

        // Act
        sut.DeleteComment(comment.Guid);

        // Assert
        Assert.IsFalse(post.Comments.Contains(comment));
        Assert.IsNull(uow.Repository<Comment>().Get(comment.Guid));
    }


    bool AreEqual(Comment c, Student author, Post post, string content)
    {
        return c.Author == author && c.Post == post && c.Content == content;
    }
}
