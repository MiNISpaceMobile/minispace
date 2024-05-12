using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class StudentService(IUnitOfWork uow) : BaseService<IStudentService, StudentService>(uow), IStudentService
{
    public Student GetStudent(Guid guid)
    {
        AllowAllUsers();

        Student student = uow.Repository<Student>().GetOrThrow(guid);

        return student;
    }

    public Student CreateStudent(string firstName, string lastName, string email, string? externalId = null)
    {
        AllowOnlyNotLoggedIn();

        Student student = new Student(firstName, lastName, email, externalId);

        uow.Repository<Student>().Add(student);
        uow.Commit();

        return student;
    }

    public void UpdateStudent(Student newStudent)
    {
        AllowOnlyStudents();

        Student student = (Student)ActingUser!;

        student.FirstName = newStudent.FirstName;
        student.LastName = newStudent.LastName;
        student.Email = newStudent.Email;

        student.Description = newStudent.Description;
        student.DateOfBirth = newStudent.DateOfBirth;

        student.EmailNotification = newStudent.EmailNotification;
        student.IsOrganizer = newStudent.IsOrganizer;

        uow.Commit();
    }

    public void DeleteStudent()
    {
        AllowOnlyStudents();

        Student student = (Student)ActingUser!;

        uow.Repository<Student>().Delete(student);
        uow.Commit();
    }

    public FriendRequest? SendFriendRequest(Guid targetId)
    {
        AllowOnlyStudents();

        Student author = (Student)ActingUser!;

        if (author.Guid == targetId)
            throw new InvalidOperationException("Can't befriend self");

        Student target = uow.Repository<Student>().GetOrThrow(targetId);

        FriendRequest? opposite = target.SentFriendRequests.Where(r => r.Target == author).SingleOrDefault();
        if (opposite is not null) // Author already received a FriendRequest from target
        {
            target.Friends.Add(author);
            author.Friends.Add(target);

            uow.Repository<FriendRequest>().Delete(opposite);
            uow.Commit();

            return null;
        }

        if (target.Friends.Contains(author))
            throw new InvalidOperationException("Already friends");

        if (target.ReceivedFriendRequests.Any(r => r.Author == author))
            throw new InvalidOperationException("Already sent a friend request");

        FriendRequest request = new FriendRequest(target, author);
        uow.Repository<FriendRequest>().Add(request);
        uow.Commit();

        return request;
    }

    public void RespondFriendRequest(Guid requestId, bool accept)
    {
        FriendRequest request = uow.Repository<FriendRequest>().GetOrThrow(requestId);

        AllowOnlyUser(request.Target);

        if (accept)
        {
            Student target = request.Target;
            Student author = request.Author;

            target.Friends.Add(author);
            author.Friends.Add(target);
        }

        uow.Repository<FriendRequest>().Delete(request);
        uow.Commit();
    }
}
