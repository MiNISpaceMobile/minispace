using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IPostService : IBaseService<IPostService>
{
    public Post CreatePost(Guid eventGuid, string content);
    public Post GetPost(Guid guid);
    public void DeletePost(Guid guid);
    public string GetPicturesModificationUrl(Guid postGuid);
}
