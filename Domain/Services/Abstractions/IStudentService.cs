using Domain.DataModel;

namespace Domain.Services.Abstractions
{
    public interface IStudentService
    {
        public Student GetStudent(Guid guid);

        public Student CreateStudent(string firstName, string lastName, string email);

        public void UpdateStudent(Student newStudent);

        public void DeleteStudent(Guid guid);

        public void SendFriendRequest(Guid targetId, Guid sourceId);

        public void RespondFriendRequest(Guid requestId, bool accept);
    }
}
