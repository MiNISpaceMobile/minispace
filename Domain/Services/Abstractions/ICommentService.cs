using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ICommentService
    {
        public Comment CreateComment(Guid authorGuid, Guid postGuid, string content, Guid inResponseToGuid, DateTime? creationDate = null);
        public Comment GetComment(Guid guid);
        public void DeleteComment(Guid guid);
    }
}
