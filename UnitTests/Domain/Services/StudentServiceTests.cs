using Domain.DataModel;
using Domain.Services.Implementations;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class StudentServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private Student[] sts;
    private StudentService sut;
    private DictionaryUnitOfWork uow;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        Student st0 = new Student("st0", "st0", "st0");
        Student st1 = new Student("st1", "st1", "st1");
        Student st2 = new Student("st2", "st2", "st2");
        st0.Friends.Add(st2);
        st2.Friends.Add(st0);
        sts = [st0, st1, st2];

        uow = new DictionaryUnitOfWork(sts);
        sut = new StudentService(uow);
    }

    #region GetStudent
    [TestMethod]
    public void GetStudent_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.GetStudent(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void GetStudent_Correct_ReturnsStudent()
    {
        var result = sut.GetStudent(sts[1].Guid);

        Assert.AreSame(sts[1], result);
        Assert.AreEqual(0, uow.CommitCount);
    }
    #endregion GetStudent

    #region CreateStudent
    [TestMethod]
    public void CreateStudent_Always_StudentCreated()
    {
        var result = sut.CreateStudent("test", "test", "test");

        Assert.IsNotNull(result);
        Assert.AreEqual(4, uow.Repository<Student>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion CreateStudent

    #region UpdateStudent
    [TestMethod]
    public void UpdateStudent_Nonexistent_ThrowsInvalidGuid()
    {
        var newStudent = new Student("test", "test", "test") { Guid = Guid.NewGuid() };

        var act = () => sut.UpdateStudent(newStudent);

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UpdateStudent_Correct_StudentUpdated()
    {
        var newStudent = new Student("test", "test", "test")
        {
            Guid = sts[1].Guid,
            DateOfBirth = DateTime.UnixEpoch,
        };

        sut.UpdateStudent(newStudent);

        Assert.AreEqual("test", sts[1].FirstName);
        Assert.AreEqual(DateTime.UnixEpoch, sts[1].DateOfBirth);
        Assert.AreEqual(3, uow.Repository<Student>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UpdateStudent

    #region DeleteStudent
    [TestMethod]
    public void DeleteStudent_Nonexistent_ThrowsInvalidGuid()
    {
        var act = () => sut.DeleteStudent(Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteStudent_Correct_StudentDeleted()
    {
        sut.DeleteStudent(sts[0].Guid);

        Assert.AreEqual(2, uow.Repository<Student>().GetAll().Count());
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion DeleteStudent

    #region SendFriendRequest
    [TestMethod]
    public void SendFriendRequest_BothNonexistent_ThrowsInvalidGuid()
    {
        Action act = () => sut.SendFriendRequest(Guid.NewGuid(), Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_TargetNonexistent_ThrowsInvalidGuid()
    {
        Action act = () => sut.SendFriendRequest(Guid.NewGuid(), sts[1].Guid);

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_AuthorNonexistent_ThrowsInvalidGuid()
    {
        Action act = () => sut.SendFriendRequest(sts[0].Guid, Guid.NewGuid());

        Assert.ThrowsException<InvalidGuidException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_AlreadySent_ThrowsInvalidOperation()
    {
        sut.SendFriendRequest(sts[0].Guid, sts[1].Guid);

        var act = () => sut.SendFriendRequest(sts[0].Guid, sts[1].Guid);

        Assert.ThrowsException<InvalidOperationException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void SendFriendRequest_AlreadyFriends_ThrowsInvalidOperation()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void SendFriendRequest_OppositeAlreadySent_MakesFriends()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void SendFriendRequest_Correct_FriendRequestCreated()
    {
        throw new NotImplementedException();
    }
    #endregion SendFriendRequest

    #region RespondFriendRequest
    // TODO: Tests
    //[TestMethod]
    //public void TrySetFriendship_MakeFriends_MakesFriends()
    //{
    //    sut.TrySetFriendship(sts[0].Guid, sts[2].Guid, false);

    //    Assert.IsTrue(sts[0].Friends.Contains(sts[2]));
    //    Assert.IsTrue(sts[2].Friends.Contains(sts[0]));
    //    Assert.AreEqual(1, uow.CommitCount);
    //}
    #endregion RespondFriendRequest
}
