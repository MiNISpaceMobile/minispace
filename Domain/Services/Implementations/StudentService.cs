using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Abstractions;

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

    public bool TrySetFriendship(Guid guid1, Guid guid2, bool make_friends)
    {
        // 2 students, 1 query
        var students = uow.Repository<Student>().GetAll().Where(s => s.Guid == guid1 || s.Guid == guid2);

        if (students.Count() != 2)
            throw new InvalidGuidException();

        Student student1 = students.First();
        Student student2 = students.Last();

        if (make_friends)
        {
            if (student1.Friends.Contains(student2))
                return false;

            student1.Friends.Add(student2);
            student2.Friends.Add(student1);
        }
        else
        {
            if (!student1.Friends.Contains(student2))
                return false;

            student1.Friends.Remove(student2);
            student2.Friends.Remove(student1);
        }
        uow.Commit();

        return true;
    }
}
