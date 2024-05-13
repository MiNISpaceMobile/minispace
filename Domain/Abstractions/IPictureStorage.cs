namespace Domain.Abstractions;

public interface IPictureStorage
{
    public string PictureStorageUrl { get; }
    public string ProfilePictureModificationUrl(Guid userGuid);
    public string EventPicturesModificationUrl(Guid eventGuid);
    public string PostPicturesModificationUrl(Guid postGuid);
}
