using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IPictureService : IBaseService<IPictureService>
{
    public string UploadUserProfilePicture(Stream file);
    public void DeleteUserProfilePicture();

    public string UploadEventPicture(Guid eventGuid, int index, Stream file);
    public void DeleteEventPicture(Guid eventGuid, int index);

    public string UploadPostPicture(Guid postGuid, int index, Stream file);
    public void DeletePostPicture(Guid postGuid, int index);
}
