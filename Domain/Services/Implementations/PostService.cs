using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public class PostService(IUnitOfWork uow, IStorage storage)
    : BaseService<IPostService, PostService>(uow), IPostService
{
    public Post CreatePost(Guid eventGuid, string content)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (content == string.Empty)
            throw new EmptyContentException();

        var post = new Post(@event.Organizer!, @event, content, DateTime.Now);
        uow.Repository<Post>().Add(post);
        @event.Posts.Add(post);

        uow.Commit();
        return post;
    }

    public void DeletePost(Guid guid)
    {
        Post post = uow.Repository<Post>().GetOrThrow(guid);

        AllowOnlyUser(post.Author);

        uow.Repository<Post>().TryDelete(guid);
        post.Event.Posts.Remove(post);

        uow.Commit();

        // Remove all pictures and other potential related files
        storage.TryDeleteDirectory(IStorage.PostDirectory(guid));
    }

    public Post GetPost(Guid guid)
    {
        AllowEveryone();

        Post? post = uow.Repository<Post>().Get(guid);
        if (post is null)
            throw new InvalidGuidException<Post>();

        return post;
    }

    public List<Post> GetUsersPosts()
    {
        AllowOnlyLoggedIn();

        List<Post> posts = new List<Post>();
        foreach (var e in ActingUser!.SubscribedEvents.AsEnumerable())
            posts.AddRange(e.Posts);

        return posts;
    }
}
