using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions
{
    public interface IStudentService : IBaseService<IStudentService>
    {
        public Student GetStudent(Guid guid);

        public Student CreateStudent(string firstName, string lastName, string email);

        public void UpdateStudent(Student newStudent);

        public void DeleteStudent(Guid guid);

        /* Returns newly created FriendRequest
         * unless an opposite FriendRequest was already pending
         * in which case it is accepted, deleted and function returns null
         */
        public FriendRequest? SendFriendRequest(Guid targetId, Guid sourceId);

        public void RespondFriendRequest(Guid requestId, bool accept);
    }
}
