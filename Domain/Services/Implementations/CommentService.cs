using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class CommentService(IUnitOfWork uow) : BaseService<ICommentService, CommentService>(uow), ICommentService
{
    public Comment CreateComment(Guid authorGuid, Guid postGuid, string content, Guid inResponseToGuid = new Guid(), DateTime? creationDate = null)
    {
        Student author = uow.Repository<Student>().GetOrThrow(authorGuid);

        AllowUser(author);

        Post post = uow.Repository<Post>().GetOrThrow(postGuid);
        Comment? inResponseTo = uow.Repository<Comment>().Get(inResponseToGuid);
        if (inResponseToGuid != Guid.Empty && inResponseTo is null)
            throw new InvalidGuidException<Comment>();
        if (content == string.Empty)
            throw new EmptyContentException();

        Comment comment = new Comment(author, post, content, inResponseTo, creationDate);
        uow.Repository<Comment>().Add(comment);
        post.Comments.Add(comment);

        uow.Commit();
        return comment;
    }

    public void DeleteComment(Guid guid)
    {
        Comment comment = uow.Repository<Comment>().GetOrThrow(guid);

        AllowUser(comment.Author);

        uow.Repository<Comment>().TryDelete(guid);
        comment.Post.Comments.Remove(comment);

        uow.Commit();
    }

    public Comment GetComment(Guid guid)
    {
        AllowEveryone();

        Comment? comment = uow.Repository<Comment>().Get(guid);
        if (comment is null)
            throw new InvalidGuidException<Comment>();

        return comment;
    }
}
