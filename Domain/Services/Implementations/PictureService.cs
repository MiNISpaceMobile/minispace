using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class PictureService(IUnitOfWork uow, IStorage storage, IPictureHandler pictureHandler)
    : BaseService<IPictureService, PictureService>(uow), IPictureService
{
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public int MaxPicturesPerEvent { get; set; } = 10;
    public int MaxPicturesPerPost { get; set; } = 3;

    private string PictureIdFilename(int id) => $"{id}{pictureHandler.Extension}";
    private string ProfilePictureFilename => $"profile{pictureHandler.Extension}";

    public string UploadUserProfilePicture(Stream source)
    {
        AllowAllUsers();

        if (source.Length > MaxFileSize)
            throw new FileTooBigException();

        Stream picture;
        try
        {
            picture = pictureHandler.ConvertIfNeeded(source);
        }
        catch
        {
            throw new FileFormatException();
        }

        try
        {
            ActingUser!.ProfilePictureUrl =
                storage.UploadFile(picture, IStorage.UserDirectory(ActingUser.Guid), ProfilePictureFilename);
        }
        catch
        {
            throw new StorageException();
        }
        finally
        {
            if (!ReferenceEquals(picture, source))
                picture.Dispose();
        }

        uow.Commit();

        return ActingUser.ProfilePictureUrl;
    }

    public void DeleteUserProfilePicture()
    {
        AllowAllUsers();

        try
        {
            storage.DeleteFile(IStorage.UserDirectory(ActingUser!.Guid), ProfilePictureFilename);
        }
        catch
        {
            throw new StorageException();
        }

        ActingUser.ProfilePictureUrl = null;
        uow.Commit();
    }

    public string UploadEventPicture(Guid eventGuid, int index, Stream source)
    {
        if (source.Length > MaxFileSize)
            throw new FileTooBigException();

        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (index < 0 || index > @event.Pictures.Count)
            throw new FileIndexException();

        if (index == MaxPicturesPerEvent)
            throw new FileLimitExeption();

        Stream picture;
        try
        {
            picture = pictureHandler.ConvertIfNeeded(source);
        }
        catch
        {
            throw new FileFormatException();
        }

        string url;
        try
        {
            // Inserts uploaded picture at given index -> some pictures' indices need to be incremented
            foreach (var eventPicture in @event.Pictures)
                if (eventPicture.Index >= index)
                    eventPicture.Index++;

            // Calculate Id that was not used by previous pictures
            int nextPictureId = @event.Pictures.Select(x => x.Id).DefaultIfEmpty().Max() + 1;

            url = storage.UploadFile(picture, IStorage.EventDirectory(eventGuid), PictureIdFilename(nextPictureId));
            @event.Pictures.Add(new EventPicture(@event, index, url));
        }
        catch
        {
            throw new StorageException();
        }
        finally
        {
            if (!ReferenceEquals(picture, source))
                picture.Dispose();
        }

        uow.Commit();

        return url;
    }

    public void DeleteEventPicture(Guid eventGuid, int index)
    {
        Event @event = uow.Repository<Event>().GetOrThrow(eventGuid);

        AllowOnlyUser(@event.Organizer);

        if (index < 0 || index >= @event.Pictures.Count)
            throw new FileIndexException();

        EventPicture picture = @event.Pictures.Single(x => x.Index == index);

        try
        {
            storage.DeleteFile(IStorage.EventDirectory(eventGuid), PictureIdFilename(picture.Id));
        }
        catch
        {
            throw new StorageException();
        }

        // Deletes picture at given index -> some pictures' indices need to be decremented
        foreach (var eventPicture in @event.Pictures)
            if (eventPicture.Index >= index)
                eventPicture.Index--;

        @event.Pictures.Remove(picture);
        uow.Commit();
    }

    public string UploadPostPicture(Guid postGuid, int index, Stream source)
    {
        if (source.Length > MaxFileSize)
            throw new FileTooBigException();

        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        if (index < 0 || index > post.Pictures.Count)
            throw new FileIndexException();

        if (index == MaxPicturesPerPost)
            throw new FileLimitExeption();

        Stream picture;
        try
        {
            picture = pictureHandler.ConvertIfNeeded(source);
        }
        catch
        {
            throw new FileFormatException();
        }

        string url;
        try
        {
            // Inserts uploaded picture at given index -> some pictures' indices need to be incremented
            foreach (var eventPicture in post.Pictures)
                if (eventPicture.Index >= index)
                    eventPicture.Index++;

            // Calculate Id that was not used by previous pictures
            int nextPictureId = post.Pictures.Select(x => x.Id).DefaultIfEmpty().Max() + 1;

            url = storage.UploadFile(picture, IStorage.PostDirectory(postGuid), PictureIdFilename(nextPictureId));
            post.Pictures.Add(new PostPicture(post, index, url));
        }
        catch
        {
            throw new StorageException();
        }
        finally
        {
            if (!ReferenceEquals(picture, source))
                picture.Dispose();
        }

        uow.Commit();

        return url;
    }

    public void DeletePostPicture(Guid postGuid, int index)
    {
        Post post = uow.Repository<Post>().GetOrThrow(postGuid);

        AllowOnlyUser(post.Author);

        if (index < 0 || index >= post.Pictures.Count)
            throw new FileIndexException();

        PostPicture picture = post.Pictures.Single(x => x.Index == index);

        try
        {
            storage.DeleteFile(IStorage.PostDirectory(postGuid), PictureIdFilename(picture.Id));
        }
        catch
        {
            throw new StorageException();
        }

        // Deletes picture at given index -> some pictures' indices need to be decremented
        foreach (var eventPicture in post.Pictures)
            if (eventPicture.Index >= index)
                eventPicture.Index--;

        post.Pictures.Remove(picture);
        uow.Commit();
    }
}
