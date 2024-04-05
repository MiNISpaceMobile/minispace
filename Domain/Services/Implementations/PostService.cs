using Domain.Abstractions;
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

    public Post CreatePost(Student author, Event @event, string content)
    {
        if (author is null || @event is null || content == string.Empty)
            throw new ArgumentException("Arguments must not be empty");

        var post = new Post(author, @event, content, DateTime.Now);
        uow.Repository<Post>().Add(post);
        @event.Posts.Add(post);

        uow.Commit();
        return post;
    }

    public Post GetPost(Guid guid)
    {
        Post post = uow.Repository<Post>().Get(guid);
        if (post is null)
            throw new ArgumentException("Nonexistent post");

        return post;
    }
}
