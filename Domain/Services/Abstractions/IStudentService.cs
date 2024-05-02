using Domain.DataModel;

namespace Domain.Services.Abstractions
{
    public interface IStudentService
    {
        public Student GetStudent(Guid guid);

        public Student CreateStudent(string firstName, string lastName, string email, string? externalId = null);

        public void UpdateStudent(Student newStudent);

        public void DeleteStudent(Guid guid);

        public bool TrySetFriendship(Guid guid1, Guid guid2, bool make_friends);
    }
}
