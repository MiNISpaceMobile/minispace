﻿using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services
{
    public interface ICommentService : IBaseService<ICommentService>
    {
        public Comment CreateComment(Guid postGuid, string content, Guid inResponseToGuid = new Guid(), DateTime? creationDate = null);
        public Comment GetComment(Guid guid);
        public void DeleteComment(Guid guid);
        public void SetLike(Guid commentGuid, bool? isDislike);
    }
}
