using Domain.Abstractions;
using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public class CommentService : ICommentService
{
    private IUnitOfWork uow;

    public CommentService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

    public Comment CreateComment(Guid authorGuid, Guid postGuid, string content, Guid inResponseToGuid = new Guid(), DateTime? creationDate = null)
    {
        Student author = uow.Repository<Student>().Get(authorGuid);
        Post post = uow.Repository<Post>().Get(postGuid);
        Comment inResponseTo = uow.Repository<Comment>().Get(inResponseToGuid);
        if (author is null || post is null || (inResponseToGuid != Guid.Empty && inResponseTo is null))
            throw new ArgumentException("Nonexistent object");
        if (content == string.Empty)
            throw new ArgumentException("Arguments must not be empty");

        Comment comment = new Comment(author, post, content, inResponseTo, creationDate);
        uow.Repository<Comment>().Add(comment);
        post.Comments.Add(comment);

        uow.Commit();
        return comment;
    }
}
