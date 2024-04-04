using Domain.Abstractions;
using Domain.DataModel;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Services;

public class UserService
{
    public enum Error
    {
        None = 0,
        Success = 0,
        NothingChanged = 1,
        InvalidGuid,
        InvalidPassword,
        AlreadyTaken,
    }

    private IUnitOfWork uow;

    public UserService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    #region Admin

    public Error GetAdmin(Guid guid, out Administrator? administrator)
    {
        administrator = uow.Repository<Administrator>().Get(guid);

        if (administrator is null)
            return Error.InvalidGuid;

        return Error.None;
    }

    public Error AddAdmin(string username, string email, string password)
    {
        Administrator administrator = new Administrator(username, email, password);

        uow.Repository<Administrator>().Add(administrator);
        uow.Commit();

        return Error.None;
    }

    #endregion Admin

    #region Student

    public Error GetStudent(Guid guid, out Student? student)
    {
        student = uow.Repository<Student>().Get(guid);

        if (student is null)
            return Error.InvalidGuid;

        return Error.None;
    }

    public Error AddStudent(string username, string email, string password, out Guid? guid)
    {
        guid = null;

        if (uow.Repository<Student>().GetAll().Any(u => u.Username == username || u.Email == email))
            return Error.AlreadyTaken;

        Student student = new Student(username, email, password);

        uow.Repository<Student>().Add(student);
        uow.Commit();

        guid = student.Guid;

        return Error.None;
    }

    public Error DeleteStudent(Guid guid, string password)
    {
        Student? student = uow.Repository<Student>().Get(guid);

        if (student is null)
            return Error.InvalidGuid;

        if (!student.CheckPassword(password))
            return Error.InvalidPassword;

        uow.Repository<Student>().TryDelete(guid);
        uow.Commit();

        return Error.None;
    }

    public Error SetFriendship(Guid guid1, Guid guid2, bool friends)
    {
        var students = uow.Repository<Student>().GetAll().Where(s => s.Guid == guid1 || s.Guid == guid2);

        if (students.Count() != 2)
            return Error.InvalidGuid;

        Student student1 = students.First();
        Student student2 = students.Last();

        if (student1.Friends.Contains(student2) == friends)
            return Error.NothingChanged;

        if (friends)
        {
            student1.Friends.Add(student2);
            student2.Friends.Add(student1);
        }
        else
        {
            student1.Friends.Remove(student2);
            student2.Friends.Remove(student1);
        }
        uow.Commit();

        return Error.None;
    }

    public Error SetDescription(Guid guid, string description)
    {
        Student? student = uow.Repository<Student>().Get(guid);

        if (student is null)
            return Error.InvalidGuid;

        if (student.Description == description)
            return Error.NothingChanged;

        student.Description = description;
        uow.Commit();

        return Error.None;
    }

    public Error SetDateOfBirth(Guid guid, DateTime? dateOfBirth)
    {
        Student? student = uow.Repository<Student>().Get(guid);

        if (student is null)
            return Error.InvalidGuid;

        if ((!student.DateOfBirth.HasValue && !dateOfBirth.HasValue) ||
            (DateOnly.FromDateTime(student.DateOfBirth!.Value) == DateOnly.FromDateTime(dateOfBirth!.Value)))
            return Error.NothingChanged;

        student.DateOfBirth = dateOfBirth;
        uow.Commit();

        return Error.None;
    }

    public Error SetNotificationPreferences(Guid guid, bool email)
    {
        Student? student = uow.Repository<Student>().Get(guid);

        if (student is null)
            return Error.InvalidGuid;

        if (student.EmailNotification == email)
            return Error.NothingChanged;

        student.EmailNotification = email;
        uow.Commit();

        return Error.None;
    }

    public Error SetIsOrganizer(Guid guid, bool isOrganizer)
    {
        Student? student = uow.Repository<Student>().Get(guid);

        if (student is null)
            return Error.InvalidGuid;

        if (student.IsOrganizer == isOrganizer)
            return Error.NothingChanged;

        student.IsOrganizer = isOrganizer;
        uow.Commit();

        return Error.None;
    }

    #endregion Student

    #region User

    public Error SetUsername(Guid guid, string username)
    {
        User? user = uow.Repository<User>().Get(guid);

        if (user is null)
            return Error.InvalidGuid;

        if (user.Username == username)
            return Error.NothingChanged;

        if (uow.Repository<User>().GetAll().Any(u => u.Username == username))
            return Error.AlreadyTaken;

        user.Username = username;
        uow.Commit();

        return Error.None;
    }

    public Error SetEmail(Guid guid, string email)
    {
        User? user = uow.Repository<User>().Get(guid);

        if (user is null)
            return Error.InvalidGuid;

        if (user.Email == email)
            return Error.NothingChanged;

        if (uow.Repository<User>().GetAll().Any(u => u.Email == email))
            return Error.AlreadyTaken;

        user.Email = email;
        uow.Commit();

        return Error.None;
    }

    public Error SetPassword(Guid guid, string oldpassword, string password)
    {
        User? user = uow.Repository<User>().Get(guid);

        if (user is null)
            return Error.InvalidGuid;

        if (!user.CheckPassword(oldpassword))
            return Error.InvalidPassword;

        if (!user.UpdatePassword(password))
            return Error.NothingChanged;

        return Error.None;
    }

    // TODO: Implement when picture storage is ready
    public Error SetProfilePicture(Guid guid, string? picture)
    {
        throw new NotImplementedException();
    }

    #endregion User
}
