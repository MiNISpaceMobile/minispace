using Domain.DataModel;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Infrastucture.Repositories;

[TestClass]
public class DictionaryRepositoryTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private Administrator ad0;
    private Administrator ad1;

    private DictionaryUnitOfWork uow;
    private DictionaryRepository<Administrator> sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        ad0 = new Administrator("ad0", "ad0", "ad0");
        ad1 = new Administrator("ad1", "ad1", "ad1");

        uow = new DictionaryUnitOfWork([ad0, ad1]);
        sut = (DictionaryRepository<Administrator>)uow.Repository<Administrator>();
    }

    #region Add
    [TestMethod]
    public void Add_PresetGuid_AddsKeepingGuid()
    {
        Guid guid = Guid.NewGuid();
        Administrator ad = new Administrator("ad", "ad", "ad") { Guid = guid };

        sut.Add(ad);

        Assert.AreEqual(guid, ad.Guid);
        Assert.AreSame(ad, uow.Tables[typeof(Administrator)][guid]);
    }

    [TestMethod]
    public void Add_GuidEmpty_AddsAssigningGuid()
    {
        Administrator ad = new Administrator("ad", "ad", "ad");

        sut.Add(ad);

        Assert.AreNotEqual(Guid.Empty, ad.Guid);
        Assert.AreSame(ad, uow.Tables[typeof(Administrator)][ad.Guid]);
    }
    #endregion Add

    #region Get
    [TestMethod]
    public void Get_InvalidGuid_ReturnsNull()
    {
        var ad = sut.Get(Guid.NewGuid());

        Assert.IsNull(ad);
    }

    [TestMethod]
    public void Get_Correct_ReturnsNotNull()
    {
        var ad = sut.Get(ad0.Guid);

        Assert.IsNotNull(ad);
        Assert.AreSame(ad0, ad);
    }
    #endregion Get

    #region GetAll
    [TestMethod]
    public void GetAll_Always_ReturnsAll()
    {
        var ads = sut.GetAll();

        Assert.AreEqual(2, ads.Count());
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
        bool deleted = sut.TryDelete(ad1.Guid);

        Assert.IsTrue(deleted);
        Assert.IsFalse(uow.Tables[typeof(Administrator)].ContainsKey(ad1.Guid));
    }
    #endregion TryDelete

    #region DeleteAll
    [TestMethod]
    public void DeleteAll_Always_DeleteAllMatchingPredicate()
    {
        int count = sut.DeleteAll(ad => string.Equals(ad.Email, "ad0"));

        Assert.AreEqual(1, count);
        Assert.AreEqual(1, uow.Tables[typeof(Administrator)].Count);
        Assert.IsFalse(uow.Tables[typeof(Administrator)].ContainsKey(ad0.Guid));
    }
    #endregion DeleteAll
}
