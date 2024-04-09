using Domain.DataModel;
using Domain.Services;
using Domain.Services.Implementations;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class UserServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private Student st;
    private Administrator ad;

    private UserService sut;
    private DictionaryUnitOfWork uow;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        st = new Student("st", "st", "st");
        ad = new Administrator("ad", "ad", "ad");

        uow = new DictionaryUnitOfWork([st, ad]);
        sut = new UserService(uow);
    }

    #region GetUser
    [TestMethod]
    public void GetUser_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.GetUser(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void GetUser_Administrator_ReturnsAdministrator()
    {
        var result = sut.GetUser(ad.Guid);

        Assert.AreSame(ad, result);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void GetUser_Student_ReturnsStudent()
    {
        var result = sut.GetUser(st.Guid);

        Assert.AreSame(st, result);
        Assert.AreEqual(0, uow.CommitCount);
    }
    #endregion GetUser
}
