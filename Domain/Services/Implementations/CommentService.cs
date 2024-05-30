using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class CommentService(IUnitOfWork uow) : BaseService<ICommentService, CommentService>(uow), ICommentService
{
    public Comment CreateComment(Guid postGuid, string content, Guid inResponseToGuid = new Guid(), DateTime? creationDate = null)
    {
        AllowOnlyLoggedIn();

        User author = ActingUser!;

        Post post = uow.Repository<Post>().GetOrThrow(postGuid);
        Comment? inResponseTo = uow.Repository<Comment>().Get(inResponseToGuid);
        if (inResponseToGuid != Guid.Empty && inResponseTo is null)
            throw new InvalidGuidException<Comment>();
        if (content == string.Empty)
            throw new EmptyContentException();

        Comment comment = new Comment(author, post, content, inResponseTo, creationDate);
        uow.Repository<Comment>().Add(comment);

        post.Comments.Add(comment);
        if (inResponseTo is not null)
            inResponseTo.Responses.Add(comment);

        uow.Commit();
        return comment;
    }

    public void DeleteComment(Guid guid)
    {
        Comment comment = uow.Repository<Comment>().GetOrThrow(guid);

        AllowOnlyUser(comment.Author);

        uow.Repository<Comment>().TryDelete(guid);

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

    public void SetLike(Guid commentGuid, bool? isDislike)
    {
        AllowOnlyLoggedIn();

        Comment comment = uow.Repository<Comment>().GetOrThrow(commentGuid);

        Like? userLike = comment.Likes.SingleOrDefault(x => x.AuthorId == ActingUser!.Guid);
        if (userLike is null && !isDislike.HasValue)
            return;
        if (userLike is not null && isDislike.HasValue && userLike.IsDislike == isDislike.Value)
            return;

        if (userLike is null && isDislike.HasValue)
            comment.Likes.Add(new Like(ActingUser!, comment, isDislike.Value));
        else if (userLike is not null && isDislike.HasValue)
            userLike.IsDislike = isDislike.Value;
        else if (userLike is not null && !isDislike.HasValue)
            comment.Likes.Remove(userLike);

        uow.Commit();
    }
}
