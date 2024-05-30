using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IPostService : IBaseService<IPostService>
{
    public Post CreatePost(Guid eventGuid, string title, string content);
    public Post GetPost(Guid guid);
    public List<Post> GetUsersPosts();
    public void DeletePost(Guid guid);
}
