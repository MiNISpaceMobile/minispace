using Domain.BaseTypes;

namespace Domain.Services.Abstractions;

public interface IPictureService : IBaseService<IPictureService>
{
    public void UploadUserProfilePicture(Stream file);
    public void DeleteUserProfilePicture();

    public void UploadEventPicture(Guid eventGuid, Stream file);
    public void DeleteEventPicture(Guid eventGuid, int index);

    public void UploadPostPicture(Guid postGuid, Stream file);
    public void DeletePostPicture(Guid postGuid, int index);
}
