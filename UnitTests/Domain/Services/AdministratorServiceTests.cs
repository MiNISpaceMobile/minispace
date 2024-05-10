using Domain.DataModel;
using Domain.Services.Implementations;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class AdministratorServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private Administrator ad0;
    private Administrator ad1;
    private Administrator[] ads;

    private AdministratorService sut;
    private DictionaryUnitOfWork uow;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        ad0 = new Administrator("ad0", "ad0", "ad0");
        ad1 = new Administrator("ad1", "ad1", "ad1");
        ads = [ad0, ad1];

        uow = new DictionaryUnitOfWork(ads);
        sut = new AdministratorService(uow);
    }

    #region GetAdministrator
    [TestMethod]
    public void GetAdministrator_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(ad0.Guid).GetAdministrator(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException<Administrator>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void GetAdministrator_Correct_ReturnsAdministrator()
    {
        var result = sut.AsUser(ad0.Guid).GetAdministrator(ad1.Guid);

        Assert.AreSame(ads[1], result);
        Assert.AreEqual(0, uow.CommitCount);
    }
    #endregion GetAdministrator

    #region CreateAdministrator
    [TestMethod]
    public void CreateAdministrator_Always_AdministratorCreated()
    {
        var result = sut.AsUser(ads[0].Guid).CreateAdministrator("test", "test", "test");

        Assert.IsNotNull(result);
        Assert.AreEqual(3, uow.Repository<Administrator>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion CreateAdministrator

    #region UpdateAdministrator
    [TestMethod]
    public void UpdateAdministrator_Nonexistent_ThrowsInvalidGuid()
    {
        var newAdministrator = new Administrator("test", "test", "test") { Guid = Guid.NewGuid() };

        var act = () => sut.AsUser(ad0.Guid).UpdateAdministrator(newAdministrator);

        Assert.ThrowsException<InvalidGuidException<Administrator>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UpdateAdministrator_Correct_AdministratorUpdated()
    {
        var newAdministrator = new Administrator("test", "test", "test")
        {
            Guid = ad0.Guid,
        };

        sut.AsUser(ad0.Guid).UpdateAdministrator(newAdministrator);

        Assert.AreEqual("test", ads[0].FirstName);
        Assert.AreEqual(2, uow.Repository<Administrator>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UpdateAdministrator

    #region DeleteAdministrator
    [TestMethod]
    public void DeleteAdministrator_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(ad0.Guid).DeleteAdministrator(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException<Administrator>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteAdministrator_Correct_AdministratorDeleted()
    {
        sut.AsUser(ad0.Guid).DeleteAdministrator(ads[0].Guid);

        Assert.AreEqual(1, uow.Repository<Administrator>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion DeleteAdministrator
}
