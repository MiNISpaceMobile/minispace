using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;
using System.Net;
using Moq;

namespace UnitTests.Domain.Services;

[TestClass]
public class CommentServiceTests
{
    private IUnitOfWork uow;
    private List<Student> students;
    private List<Post> posts;

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

    //[TestMethod]
    //public void GetPost_NonexistentPost_ShouldThrowArgumentException()
    //{
    //    // Arrange
    //    PostService sut = new PostService(uow);

    //    // Act
    //    var action = () => sut.GetPost(new Guid());

    //    // Assert
    //    var ex = Assert.ThrowsException<ArgumentException>(action);
    //    Assert.AreEqual(ex.Message, "Nonexistent post");
    //}

    //[TestMethod]
    //public void GetPost_CorrectInput_ShouldReturnCorrectPost()
    //{
    //    // Arrange
    //    PostService sut = new PostService(uow);
    //    Event @event = events.Last();
    //    Student author = students.Last();
    //    string content = "a";
    //    Post post = new Post(author, @event, content);
    //    uow.Repository<Post>().Add(post);

    //    // Act
    //    Post result = sut.GetPost(post.Guid);

    //    // Assert
    //    Assert.AreEqual(post, result);
    //}

    //[TestMethod]
    //public void DeletePost_NonexistentPost_ShouldThrowArgumentException()
    //{
    //    // Arrange
    //    PostService sut = new PostService(uow);

    //    // Act
    //    var action = () => sut.DeletePost(new Guid());

    //    // Assert
    //    var ex = Assert.ThrowsException<ArgumentException>(action);
    //    Assert.AreEqual(ex.Message, "Nonexistent post");
    //}

    //[TestMethod]
    //public void DeletePost_CorrectInput_ShouldDeleteCorrectPost()
    //{
    //    // Arrange
    //    PostService sut = new PostService(uow);
    //    Event @event = events.Last();
    //    Student author = students.Last();
    //    string content = "a";
    //    Post post = new Post(author, @event, content);
    //    uow.Repository<Post>().Add(post);
    //    @event.Posts.Add(post);

    //    // Act
    //    sut.DeletePost(post.Guid);

    //    // Assert
    //    Assert.IsFalse(@event.Posts.Contains(post));
    //    Assert.IsNull(uow.Repository<Post>().Get(post.Guid));
    //}


    bool AreEqual(Comment c, Student author, Post post, string content)
    {
        return c.Author == author && c.Post == post && c.Content == content;
    }
}
