using Domain.BaseTypes;

namespace Domain.Services.Abstractions;

public interface IPictureService : IBaseService<IPictureService>
{
    public string GetProfilePictureModificationUrl();
    public string GetEventPicturesModificationUrl(Guid eventGuid);
    public string GetPostPicturesModificationUrl(Guid postGuid);
}
