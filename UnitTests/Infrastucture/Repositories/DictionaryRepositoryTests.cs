using Domain.Abstractions;
using Domain.DataModel;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Infrastucture.Repositories;

[TestClass]
public class DictionaryRepositoryTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private User st0;
    private User st1;

    private DictionaryUnitOfWork uow;
    private IRepository<User> sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        st0 = new User("st0", "st0", "st0", now);
        st1 = new User("st1", "st1", "st1", now);

        uow = new DictionaryUnitOfWork([st0, st1]);
        sut = (DictionaryRepository<User>)uow.Repository<User>();
    }

    #region Add
    [TestMethod]
    public void Add_PresetGuid_AddsKeepingGuid()
    {
        Guid guid = Guid.NewGuid();
        User st = new User("st", "st", "st", DateTime.Now) { Guid = guid };

        sut.Add(st);

        Assert.AreEqual(guid, st.Guid);
        Assert.AreSame(st, uow.Tables[typeof(User)][guid]);
    }

    [TestMethod]
    public void Add_GuidEmpty_AddsAssigningGuid()
    {
        User st = new User("st", "st", "st", DateTime.Now);

        sut.Add(st);

        Assert.AreNotEqual(Guid.Empty, st.Guid);
        Assert.AreSame(st, uow.Tables[typeof(User)][st.Guid]);
    }
    #endregion Add

    #region Get
    [TestMethod]
    public void Get_InvalidGuid_ReturnsNull()
    {
        var st = sut.Get(Guid.NewGuid());

        Assert.IsNull(st);
    }

    [TestMethod]
    public void Get_Correct_ReturnsNotNull()
    {
        var st = sut.Get(st0.Guid);

        Assert.IsNotNull(st);
        Assert.AreSame(st0, st);
    }
    #endregion Get

    #region GetAll
    [TestMethod]
    public void GetAll_Always_ReturnsAll()
    {
        var sts = sut.GetAll();

        Assert.AreEqual(2, sts.Count());
    }
    #endregion GetAll

    #region TryDelete
    [TestMethod]
    public void TryDelete_InvalidGuid_ReturnsFalse()
    {
        bool deleted = sut.TryDelete(Guid.NewGuid());
        Assert.IsFalse(deleted);
    }

    [TestMethod]
    public void TryDelete_Correct_DeletesReturnsTrue()
    {
        bool deleted = sut.TryDelete(st1.Guid);

        Assert.IsTrue(deleted);
        Assert.IsFalse(uow.Tables[typeof(User)].ContainsKey(st1.Guid));
    }
    #endregion TryDelete

    #region DeleteAll
    [TestMethod]
    public void DeleteAll_Always_DeleteAllMatchingPredicate()
    {
        int count = sut.DeleteAll(st => string.Equals(st.Email, "st0"));

        Assert.AreEqual(1, count);
        Assert.AreEqual(1, uow.Tables[typeof(User)].Count);
        Assert.IsFalse(uow.Tables[typeof(User)].ContainsKey(st0.Guid));
    }
    #endregion DeleteAll
}
