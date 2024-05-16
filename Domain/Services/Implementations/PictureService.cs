using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class PictureService(IUnitOfWork uow, IStorage storage, IPictureHandler pictureHandler)
    : BaseService<IPictureService, PictureService>(uow), IPictureService
{
    // TODO: Tests

    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public int MaxPicturesPerEvent { get; set; } = 10;
    public int MaxPicturesPerPost { get; set; } = 3;

    private string UserDirectory(Guid guid) => $"user-{guid}";
    private string EventDirectory(Guid guid) => $"event-{guid}";
    private string PostDirectory(Guid guid) => $"post-{guid}";
    private string IndexFilename(int index) => $"{index}{pictureHandler.Extension}";

    private string ProfilePictureFilename => $"profile{pictureHandler.Extension}";

    public void UploadUserProfilePicture(Stream source)
    {
        AllowAllUsers();

        if (source.Length > MaxFileSize)
            throw new FileTooBigException();

        Stream picture;
        try
        { picture = pictureHandler.ConvertIfNeeded(source); }
        catch
        { throw new FileFormatException(); }

        try
        { storage.UploadFile(picture, UserDirectory(ActingUser!.Guid), ProfilePictureFilename); }
        catch
        { throw new StorageException(); }

        ActingUser.HasProfilePicture = true;
        uow.Commit();
    }

    public void DeleteUserProfilePicture()
    {
        AllowAllUsers();

        try
        { storage.DeleteFile(UserDirectory(ActingUser!.Guid), ProfilePictureFilename); }
        catch
        { throw new StorageException(); }

        ActingUser.HasProfilePicture = false;
        uow.Commit();
    }

    public void UploadEventPicture(Guid eventGuid, Stream source)
    {
        if (source.Length > MaxFileSize)
            throw new FileTooBigException();

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (@event.PictureCount == MaxPicturesPerEvent)
            throw new FileLimitExeption();

        Stream picture;
        try
        { picture = pictureHandler.ConvertIfNeeded(source); }
        catch
        { throw new FileFormatException(); }

        try
        { storage.UploadFile(picture, EventDirectory(eventGuid), IndexFilename(@event.PictureCount)); }
        catch
        { throw new StorageException(); }

        @event.PictureCount++;
        uow.Commit();
    }

    public void DeleteEventPicture(Guid eventGuid, int index)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (index < 0 || index >= @event.PictureCount)
            throw new FileIndexException();

        var directory = EventDirectory(eventGuid);
        try
        { storage.DeleteFile(directory, IndexFilename(index)); }
        catch
        { throw new StorageException(); }

        @event.PictureCount--;
        uow.Commit();

        for (int i = index; i < @event.PictureCount; i++)
            try
            { storage.RenameFile(directory, IndexFilename(i + 1), IndexFilename(i)); }
            catch
            { throw new StorageException(); }
    }

    public void UploadPostPicture(Guid postGuid, Stream source)
    {
        if (source.Length > MaxFileSize)
            throw new FileTooBigException();

        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        if (post.PictureCount == MaxPicturesPerPost)
            throw new FileLimitExeption();

        Stream picture;
        try
        { picture = pictureHandler.ConvertIfNeeded(source); }
        catch
        { throw new FileFormatException(); }

        try
        { storage.UploadFile(picture, PostDirectory(postGuid), IndexFilename(post.PictureCount)); }
        catch
        { throw new StorageException(); }

        post.PictureCount++;
        uow.Commit();
    }

    public void DeletePostPicture(Guid postGuid, int index)
    {
        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        if (index < 0 || index >= post.PictureCount)
            throw new FileIndexException();

        var directory = PostDirectory(postGuid);
        try
        { storage.DeleteFile(directory, IndexFilename(index)); }
        catch
        { throw new StorageException(); }

        post.PictureCount--;
        uow.Commit();

        for (int i = index; i < post.PictureCount; i++)
            try
            { storage.RenameFile(directory, IndexFilename(i + 1), IndexFilename(i)); }
            catch
            { throw new StorageException(); }
    }
}
