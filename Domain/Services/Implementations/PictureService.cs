using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;
using SixLabors.ImageSharp;

namespace Domain.Services.Implementations;

public class PictureService(IUnitOfWork uow, IStorage pictureStorage) : BaseService<IPictureService, PictureService>(uow), IPictureService
{
    // TODO: Nicer exceptions
    // TODO: Tests

    public const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    public const int MaxPicturesPerEvent = 10;
    public const int MaxPicturesPerPost = 3;

    private string UserDirectory(Guid guid) => $"user-{guid}";
    private string EventDirectory(Guid guid) => $"event-{guid}";
    private string PostDirectory(Guid guid) => $"post-{guid}";
    private string IndexFilename(int index) => $"{index}.webp";

    private const string profilePictureFilename = "profile.webp";

    private Stream ConvertIfNeeded(Stream source)
    {
        if (source.Length > MaxFileSize)
            throw new Exception("File too big");

        Stream result;
        try
        {
            var format = Image.DetectFormat(source);
            if (string.Equals(format.Name, "WebP", StringComparison.InvariantCultureIgnoreCase))
                result = source;
            else // need to convert
            {
                result = new MemoryStream((int)source.Length);
                Image.Load(source).SaveAsWebp(result);
                source.Dispose();
            }
        }
        catch
        {
            throw new Exception("Invalid source format");
        }
        return result;
    }

    public void UploadUserProfilePicture(Stream source)
    {
        AllowAllUsers();

        Stream picture = ConvertIfNeeded(source);

        pictureStorage.UploadFile(picture, UserDirectory(ActingUser!.Guid), profilePictureFilename);
        picture.Dispose();

        ActingUser.HasProfilePicture = true;
        uow.Commit();
    }

    public void DeleteUserProfilePicture()
    {
        AllowAllUsers();

        pictureStorage.DeleteFile(UserDirectory(ActingUser!.Guid), profilePictureFilename);

        ActingUser.HasProfilePicture = false;
        uow.Commit();
    }

    public void UploadEventPicture(Guid eventGuid, Stream source)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (@event.PictureCount == MaxPicturesPerEvent)
            throw new Exception("Maximum number of pictures already uploaded");

        Stream picture = ConvertIfNeeded(source);

        pictureStorage.UploadFile(picture, EventDirectory(eventGuid), IndexFilename(@event.PictureCount));
        picture.Dispose();

        @event.PictureCount++;
        uow.Commit();
    }

    public void DeleteEventPicture(Guid eventGuid, int index)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (index < 0 || index >= @event.PictureCount)
            throw new Exception("Invalid result index");

        var directory = EventDirectory(eventGuid);
        pictureStorage.DeleteFile(directory, IndexFilename(index));

        @event.PictureCount--;
        uow.Commit();

        for (int i = index; i < @event.PictureCount; i++)
            pictureStorage.RenameFile(directory, IndexFilename(i + 1), IndexFilename(i));
    }

    public void UploadPostPicture(Guid postGuid, Stream source)
    {
        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        if (post.PictureCount == MaxPicturesPerPost)
            throw new Exception("Maximum number of pictures already uploaded");

        Stream picture = ConvertIfNeeded(source);

        pictureStorage.UploadFile(picture, PostDirectory(postGuid), IndexFilename(post.PictureCount));
        picture.Dispose();

        post.PictureCount++;
        uow.Commit();
    }

    public void DeletePostPicture(Guid postGuid, int index)
    {
        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        if (index < 0 || index >= post.PictureCount)
            throw new Exception("Invalid result index");

        var directory = PostDirectory(postGuid);
        pictureStorage.DeleteFile(directory, IndexFilename(index));

        post.PictureCount--;
        uow.Commit();

        for (int i = index; i < post.PictureCount; i++)
            pictureStorage.RenameFile(directory, IndexFilename(i + 1), IndexFilename(i));
    }
}
