using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services
{
    public interface ICommentService : IBaseService<ICommentService>
    {
        public Comment CreateComment(Guid authorGuid, Guid postGuid, string content, Guid inResponseToGuid, DateTime? creationDate = null);
        public Comment GetComment(Guid guid);
        public void DeleteComment(Guid guid);
    }
}
