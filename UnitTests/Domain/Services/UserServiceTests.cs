using Domain.DataModel;
using Domain.Services;
using Domain.Services.Abstractions;
using Domain.Services.Implementations;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class UserServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private User ad;
    private User st0;
    private User st1;
    private User st2;
    private FriendRequest fr;

    private IUserService sut;
    private DictionaryUnitOfWork uow;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        ad = new User("ad", "ad", "ad", now) { IsAdmin = true };
        st0 = new User("st0", "st0", "st0", now);
        st1 = new User("st1", "st1", "st1", now);
        st2 = new User("st2", "st2", "st2", now);
        st0.Friends.Add(st2);
        st2.Friends.Add(st0);

        fr = new FriendRequest(st1, st2);
        st1.ReceivedFriendRequests.Add(fr);
        st2.SentFriendRequests.Add(fr);

        uow = new DictionaryUnitOfWork([ad, st0, st1, st2, fr]);
        sut = new UserService(uow);
    }

    #region GetUsers
    [TestMethod]
    public void GetUsers_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st0.Guid).GetUsers();

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    public void GetUsers_Correct_ReturnsUsers()
    {
        var result = sut.AsUser(ad.Guid).GetUsers();

        Assert.AreEqual(4, result.Count());
        Assert.AreEqual(0, uow.CommitCount);
    }
    #endregion GetUser

    #region GetUser
    [TestMethod]
    public void GetUser_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(ad.Guid).GetUser(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException<User>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void GetUser_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st0.Guid).GetUser(st1.Guid);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void GetUser_Correct_ReturnsUser()
    {
        var result = sut.AsUser(ad.Guid).GetUser(st0.Guid);

        Assert.AreSame(st0, result);
        Assert.AreEqual(0, uow.CommitCount);
    }
    #endregion GetUser

    #region CreateUser
    [TestMethod]
    public void CreateUser_LoggedIn_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st0.Guid).CreateUser("test", "test", "test", DateTime.Now);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void CreateUser_NotLoggedIn_UserCreated()
    {
        var result = sut.CreateUser("test", "test", "test", DateTime.Now);

        Assert.IsNotNull(result);
        Assert.AreEqual(5, uow.Repository<User>().GetAll().Count());
        Assert.IsTrue(uow.Tables[typeof(User)].ContainsKey(result.Guid));
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion CreateUser

    #region UpdateUser
    [TestMethod]
    public void UpdateUser_NotLoggedIn_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(null).UpdateUser(null, null, null, null, null, null);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UpdateUser_Correct_UserUpdated()
    {
        sut.AsUser(st1.Guid).UpdateUser("test", null, null, null, DateTime.UnixEpoch, null);

        Assert.AreEqual("test", st1.FirstName);
        Assert.AreEqual(DateTime.UnixEpoch, st1.DateOfBirth);
        Assert.AreEqual(4, uow.Repository<User>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UpdateUser

    #region DeleteUser
    [TestMethod]
    public void DeleteUser_NotLoggedIn_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(null).DeleteUser();

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteUser_Correct_UserDeleted()
    {
        sut.AsUser(st0.Guid).DeleteUser();

        Assert.AreEqual(3, uow.Repository<User>().GetAll().Count());
        Assert.IsFalse(uow.Tables[typeof(User)].ContainsKey(st0.Guid));
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion DeleteUser

    #region UserRoles
    [TestMethod]
    public void UserRoles_NotAnAdmin_ThrowsUserUnauthorized()
    {
        Action act = () => sut.AsUser(st2.Guid).UserRoles(st1.Guid, null, null);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UserRoles_IsAdminNull_SetsOnlyIsOrganizer()
    {
        var result = sut.AsUser(ad.Guid).UserRoles(st0.Guid, null, true);

        Assert.AreEqual((false, true), result);
        Assert.AreEqual(st0.IsAdmin, false);
        Assert.AreEqual(st0.IsOrganizer, true);
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void UserRoles_IsOrganizerNull_SetsOnlyIsAdmin()
    {
        var result = sut.AsUser(ad.Guid).UserRoles(ad.Guid, false, null);

        Assert.AreEqual((false, false), result);
        Assert.AreEqual(ad.IsAdmin, false);
        Assert.AreEqual(ad.IsOrganizer, false);
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void UserRoles_BothNull_ChangesNothing()
    {
        var result = sut.AsUser(ad.Guid).UserRoles(ad.Guid, null, null);

        Assert.AreEqual((true, false), result);
        Assert.AreEqual(ad.IsAdmin, true);
        Assert.AreEqual(ad.IsOrganizer, false);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UserRoles_BothNotNull_SetsBoth()
    {
        var result = sut.AsUser(ad.Guid).UserRoles(st2.Guid, true, true);

        Assert.AreEqual((true, true), result);
        Assert.AreEqual(st2.IsAdmin, true);
        Assert.AreEqual(st2.IsOrganizer, true);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UserRoles

    #region SendFriendRequest
    [TestMethod]
    public void SendFriendRequest_ToSelf_ThrowsInvalidOperation()
    {
        Action act = () => sut.AsUser(st0.Guid).SendFriendRequest(st0.Guid);

        Assert.ThrowsException<FriendTargetException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_TargetNonexistent_ThrowsInvalidGuid()
    {
        Action act = () => sut.AsUser(st0.Guid).SendFriendRequest(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException<User>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_NotLoggedIn_ThrowsUserUnathorized()
    {
        Action act = () => sut.AsUser(null).SendFriendRequest(Guid.NewGuid());

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_AlreadySent_ThrowsInvalidOperation()
    {
        var act = () => sut.AsUser(st2.Guid).SendFriendRequest(st1.Guid);

        Assert.ThrowsException<FriendTargetException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_AlreadyFriends_ThrowsInvalidOperation()
    {
        var act = () => sut.AsUser(st2.Guid).SendFriendRequest(st0.Guid);

        Assert.ThrowsException<FriendTargetException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_OppositeAlreadySent_MakesFriends()
    {
        FriendRequest? request = sut.AsUser(st1.Guid).SendFriendRequest(st2.Guid);

        Assert.IsNull(request);
        Assert.IsTrue(st2.Friends.Contains(st1));
        Assert.IsTrue(st1.Friends.Contains(st2));
        Assert.AreEqual(0, uow.Repository<FriendRequest>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_Correct_FriendRequestCreated()
    {
        FriendRequest? request = sut.AsUser(st1.Guid).SendFriendRequest(st0.Guid);

        Assert.IsNotNull(request);
        Assert.AreEqual(2, uow.Repository<FriendRequest>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion SendFriendRequest

    #region RespondFriendRequest
    [TestMethod]
    public void RespondFriendRequest_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.RespondFriendRequest(Guid.NewGuid(), true);

        Assert.ThrowsException<InvalidGuidException<FriendRequest>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void RespondFriendRequest_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st2.Guid).RespondFriendRequest(fr.Guid, true);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void RespondFriendRequest_Accept_MakesFriends()
    {
        sut.AsUser(st1.Guid).RespondFriendRequest(fr.Guid, true);

        Assert.IsTrue(st1.Friends.Contains(st2));
        Assert.IsTrue(st2.Friends.Contains(st1));
        Assert.AreEqual(0, uow.Repository<FriendRequest>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }

    [TestMethod]
    public void RespondFriendRequest_Deny_DeletesRequest()
    {
        sut.AsUser(st1.Guid).RespondFriendRequest(fr.Guid, false);

        Assert.IsFalse(st1.Friends.Contains(st2));
        Assert.IsFalse(st2.Friends.Contains(st1));
        Assert.AreEqual(0, uow.Repository<FriendRequest>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion RespondFriendRequest
}
