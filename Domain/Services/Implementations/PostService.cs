using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public class PostService : IPostService
{
    private IUnitOfWork uow;

    public PostService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

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
