using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class StudentService : BaseService<StudentService>, IStudentService
{
    public StudentService(IUnitOfWork uow) : base(uow) { }

    public Student GetStudent(Guid guid)
    {
        Student student = uow.Repository<Student>().GetOrThrow(guid);

        AllowOnlyStudent(student);

        return student;
    }

    public Student CreateStudent(string firstName, string lastName, string email)
    {
        AllowNotLoggedIn();

        Student student = new Student(firstName, lastName, email);

        uow.Repository<Student>().Add(student);
        uow.Commit();

        return student;
    }

    public void UpdateStudent(Student newStudent)
    {
        Student student = uow.Repository<Student>().GetOrThrow(newStudent.Guid);

        AllowOnlyStudent(student);

        student.FirstName = newStudent.FirstName;
        student.LastName = newStudent.LastName;
        student.Email = newStudent.Email;

        student.Description = newStudent.Description;
        student.DateOfBirth = newStudent.DateOfBirth;

        student.EmailNotification = newStudent.EmailNotification;
        student.IsOrganizer = newStudent.IsOrganizer;

        uow.Commit();
    }

    public void DeleteStudent(Guid guid)
    {
        Student student = uow.Repository<Student>().GetOrThrow(guid);

        AllowOnlyStudent(student);

        uow.Repository<Student>().Delete(student);
        uow.Commit();
    }

    public FriendRequest? SendFriendRequest(Guid targetId, Guid authorId)
    {
        Student target = uow.Repository<Student>().GetOrThrow(targetId);
        Student author = uow.Repository<Student>().GetOrThrow(authorId);

        AllowOnlyStudent(author);

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

        AllowOnlyStudent(request.Target);

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
