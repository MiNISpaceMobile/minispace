using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Abstractions;
using System;

namespace Domain.Services.Implementations;

public class StudentService : IStudentService
{
    private IUnitOfWork uow;

    public StudentService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public Student GetStudent(Guid guid) => uow.Repository<Student>().GetOrThrow(guid);

    public Student CreateStudent(string firstName, string lastName, string email)
    {
        Student student = new Student(firstName, lastName, email);

        uow.Repository<Student>().Add(student);
        uow.Commit();

        return student;
    }

    public void UpdateStudent(Student newStudent)
    {
        Student student = uow.Repository<Student>().GetOrThrow(newStudent.Guid);

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
        if (!uow.Repository<Student>().TryDelete(guid))
            throw new InvalidGuidException();
        uow.Commit();
    }

    public void SendFriendRequest(Guid targetId, Guid authorId)
    {
        Student target = uow.Repository<Student>().GetOrThrow(targetId);
        Student author = uow.Repository<Student>().GetOrThrow(authorId);

        FriendRequest? opposite = target.SendFriendRequests.Where(r => r.Target == author).SingleOrDefault();
        if (opposite is not null) // Author already received a FriendRequest from target
        {
            target.Friends.Add(author);
            author.Friends.Add(target);

            uow.Repository<Notification>().TryDelete(opposite.Guid);
            uow.Commit();

            return;
        }

        if (target.Friends.Contains(author))
            throw new InvalidOperationException("Already friends");

        if (target.ReceivedFriendRequests.Any(r => r.Author == author))
            throw new InvalidOperationException("Already sent a friend request");

        uow.Repository<FriendRequest>().Add(new FriendRequest(target, author));
        uow.Commit();

        return;
    }

    public void RespondFriendRequest(Guid requestId, bool accept)
    {
        FriendRequest request = uow.Repository<FriendRequest>().GetOrThrow(requestId);

        if (accept)
        {
            Student target = request.Target;
            Student author = request.Author;

            if (target.Friends.Contains(author))
                throw new InvalidOperationException("Already friends");

            target.Friends.Add(author);
            author.Friends.Add(target);
        }

        uow.Repository<Notification>().TryDelete(requestId);
        uow.Commit();
    }
}
