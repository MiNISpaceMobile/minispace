using Domain.BaseTypes;
using Domain.DataModel;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Infrastucture.UnitOfWorks;

[TestClass]
public class DictionaryUnitOfWorkTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private DictionaryUnitOfWork sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        sut = new DictionaryUnitOfWork(Enumerable.Empty<BaseEntity>());
    }

    #region Constructor
    [TestMethod]
    public void Constructor_Always_NotNull()
    {
        DateTime now = DateTime.Now;

        var ad = new User("ad", "ad", "ad", now);
        var st = new User("st", "st", "st", now);

        var ev = new Event(st, "ev", "ev", EventCategory.Uncategorized, now, now, now, "ev", null, null);
        var po = new Post(st, ev, "po", "po");
        var co = new Comment(st, po, "co", null);

        var er = new EventReport(ev, ad, "er", "er");
        var pr = new PostReport(po, ad, "pr", "pr");
        var cr = new CommentReport(co, ad, "cr", "cr");

        sut = new DictionaryUnitOfWork([ad, st, ev, po, co, er, pr, cr]);

        Assert.IsNotNull(sut);
        Assert.AreEqual(sut.Tables.Count, 8);

        Assert.IsTrue(sut.Tables.ContainsKey(typeof(User)));
        Assert.AreEqual(2, sut.Tables[typeof(User)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(Event)));
        Assert.AreEqual(1, sut.Tables[typeof(Event)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(Post)));
        Assert.AreEqual(1, sut.Tables[typeof(Post)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(Comment)));
        Assert.AreEqual(1, sut.Tables[typeof(Comment)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(Report)));
        Assert.AreEqual(3, sut.Tables[typeof(Report)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(EventReport)));
        Assert.AreEqual(1, sut.Tables[typeof(EventReport)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(PostReport)));
        Assert.AreEqual(1, sut.Tables[typeof(PostReport)].Count);
        Assert.IsTrue(sut.Tables.ContainsKey(typeof(CommentReport)));
        Assert.AreEqual(1, sut.Tables[typeof(CommentReport)].Count);

        Assert.AreNotEqual(Guid.Empty, ad.Guid);
        Assert.AreNotEqual(Guid.Empty, st.Guid);
        Assert.AreNotEqual(Guid.Empty, ev.Guid);
        Assert.AreNotEqual(Guid.Empty, po.Guid);
        Assert.AreNotEqual(Guid.Empty, co.Guid);
        Assert.AreNotEqual(Guid.Empty, er.Guid);
        Assert.AreNotEqual(Guid.Empty, pr.Guid);
        Assert.AreNotEqual(Guid.Empty, cr.Guid);
    }
    #endregion Constructor

    #region Dispose
    [TestMethod]
    public void Dispose_Always_Disposed()
    {
        sut.Dispose();

        Assert.AreEqual(true, sut.Disposed);
    }
    #endregion Dispose

    #region Repository
    [TestMethod]
    public void Repository_NotDisposed_NotNull()
    {
        var repo = sut.Repository<User>();

        Assert.IsNotNull(repo);
    }

    [TestMethod]
    public void Repository_Disposed_ThrowsObjectDisposed()
    {
        sut.Dispose();

        var act = () => sut.Repository<User>();

        Assert.ThrowsException<ObjectDisposedException>(act);
    }
    #endregion Repository

    #region Commit
    [TestMethod]
    public void Commit_NotDisposed_CommitCounted()
    {
        sut.Commit();

        Assert.AreEqual(1, sut.CommitCount);
    }

    [TestMethod]
    public void Commit_Disposed_ThrowsObjectDisposed()
    {
        sut.Dispose();

        var act = () => sut.Commit();

        Assert.ThrowsException<ObjectDisposedException>(act);
    }
    #endregion Commit
}
