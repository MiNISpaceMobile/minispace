using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class PictureService(IUnitOfWork uow, IPictureStorage pictureStorage) : BaseService<IPictureService, PictureService>(uow), IPictureService
{
    public string GetProfilePictureModificationUrl()
    {
        AllowAllUsers();

        return pictureStorage.ProfilePictureModificationUrl(ActingUser!.Guid);
    }

    public string GetEventPicturesModificationUrl(Guid eventGuid)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        return pictureStorage.EventPicturesModificationUrl(eventGuid);
    }

    public string GetPostPicturesModificationUrl(Guid postGuid)
    {
        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        return pictureStorage.EventPicturesModificationUrl(postGuid);
    }
}
