using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class UserServiceTests
{
    private DateTime now;
    private List<Student> students;
    private List<Administrator> admins;

    private UserService service;
    private DictionaryUnitOfWork uow;

    [TestInitialize]
    public void PreTest()
    {
        now = DateTime.Now;

        Student st0 = new Student("st0", "st0", "st0") { Guid = Guid.NewGuid(), DateOfBirth = now };
        Student st1 = new Student("st1", "st1", "st1") { Guid = Guid.NewGuid() };
        students = new List<Student> { st0, st1 };

        Administrator ad0 = new Administrator("ad0", "ad0", "ad0") { Guid = Guid.NewGuid() };
        admins = new List<Administrator> { ad0 };

        uow = new DictionaryUnitOfWork(Enumerable.Concat<BaseEntity>(students, admins));

        service = new UserService(uow);
    }

    [TestMethod]
    public void GetStudent_Nonexistent_InvalidGuid()
    {
        var error = service.GetStudent(Guid.NewGuid(), out Student? student);

        Assert.AreEqual(UserService.Error.InvalidGuid, error);
        Assert.IsNull(student);
    }

    [TestMethod]
    public void GetStudent_First_Success()
    {
        var error = service.GetStudent(students[0].Guid, out Student? student);

        Assert.AreEqual(UserService.Error.Success, error);
        Assert.IsNotNull(student);
        Assert.AreEqual(students[0].Guid, student.Guid);
    }

    [TestMethod]
    public void AddStudent_UsernameTaken_AlreadyTaken()
    {
        var error = service.AddStudent("st0", "st2", "st2", out Guid? guid);

        Assert.AreEqual(UserService.Error.AlreadyTaken, error);
        Assert.IsNull(guid);
        Assert.AreEqual(2, uow.Repository<Student>().GetAll().Count());
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void AddStudent_EmailTaken_AlreadyTaken()
    {
        var error = service.AddStudent("st2", "st1", "st2", out Guid? guid);

        Assert.AreEqual(UserService.Error.AlreadyTaken, error);
        Assert.IsNull(guid);
        Assert.AreEqual(uow.Repository<Student>().GetAll().Count(), 2);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void AddStudent_NothingTaken_Success()
    {
        var error = service.AddStudent("st2", "st2", "st2", out Guid? guid);

        Assert.AreEqual(UserService.Error.Success, error);
        Assert.IsNotNull(guid);
        Assert.AreEqual(uow.Repository<Student>().GetAll().Count(), 3);
        Assert.AreEqual(1, uow.CommitCount);

        service.GetStudent(guid.Value, out Student? student);
        Assert.AreEqual("st2", student!.Username);
        Assert.AreEqual("st2", student!.Email);
    }

    [TestMethod]
    public void DeleteStudent_Nonexistent_InvalidGuid()
    {
        var error = service.DeleteStudent(Guid.NewGuid(), "st1");

        Assert.AreEqual(UserService.Error.InvalidGuid, error);
        Assert.AreEqual(uow.Repository<Student>().GetAll().Count(), 2);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteStudent_InvalidPassword_InvalidPassword()
    {
        var error = service.DeleteStudent(students[1].Guid, "st0");

        Assert.AreEqual(UserService.Error.InvalidPassword, error);
        Assert.AreEqual(uow.Repository<Student>().GetAll().Count(), 2);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteStudent_CorrectPassword_Success()
    {
        var error = service.DeleteStudent(students[0].Guid, "st0");

        Assert.AreEqual(UserService.Error.Success, error);
        Assert.AreEqual(uow.Repository<Student>().GetAll().Count(), 1);
        Assert.AreEqual(1, uow.CommitCount);
    }

    // TODO: Add more tests!
}
