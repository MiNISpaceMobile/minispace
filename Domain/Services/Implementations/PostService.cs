using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class PostService(IUnitOfWork uow) : BaseService<IPostService, PostService>(uow), IPostService
{
    public Post CreatePost(Guid authorGuid, Guid eventGuid, string content)
    {
        Student? author = uow.Repository<Student>().Get(authorGuid);
        Event? @event = uow.Repository<Event>().Get(eventGuid);
        if (author is null || @event is null)
            throw new InvalidGuidException();
        if (content == string.Empty)
            throw new EmptyContentException();

        var post = new Post(author, @event, content, DateTime.Now);
        uow.Repository<Post>().Add(post);
        @event.Posts.Add(post);

        uow.Commit();
        return post;
    }

    public void DeletePost(Guid guid)
    {
        Post? post = uow.Repository<Post>().Get(guid);
        if (post is null)
            throw new InvalidGuidException<Post>();

        uow.Repository<Post>().TryDelete(guid);
        post.Event.Posts.Remove(post);

        uow.Commit();
    }

    public Post GetPost(Guid guid)
    {
        Post? post = uow.Repository<Post>().Get(guid);
        if (post is null)
            throw new InvalidGuidException<Post>();

        return post;
    }
}
